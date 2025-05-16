using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attached to the GameObject called FoodEatRange
public class ResourceEating : MonoBehaviour
{
        public InteroceptiveAgent agent;
        private ResourceSpawner resourceSpawner;

        // public Field myArea;

        public bool isEaten;

        void Start()
        {
                // Find the ResourceSpawner in the scene
                resourceSpawner = FindObjectOfType<ResourceSpawner>();
                if (resourceSpawner == null)
                {
                Debug.LogError("ResourceSpawner not found in the scene.");
                }
        }

        // There is a sphere collider in front of the Agent, and this detects if its isTrigger is on and another collider enters.
        public void OnTriggerStay(Collider other)
        {

                if (other.CompareTag("food") || other.CompareTag("water") || other.CompareTag("pond"))
                {
                        agent.isTouched = true;
                }

                isEaten = false;
                if (agent.autoEat || agent.IsAgentActionEat)
                {
                        if (other.CompareTag("food"))
                        {
                                // agent.IncreaseLevel("food");
                                isEaten = true;
                                agent.EatenResource = true;
                                agent.EatenResourceTag = "food";
                        }
                        else if (other.CompareTag("water"))
                        {
                                // agent.IncreaseLevel("water");
                                isEaten = true;
                                agent.EatenResource = true;
                                agent.EatenResourceTag = "water";
                        }
                        else if (other.CompareTag("pond"))
                        {
                                // agent.IncreaseLevel("water");
                                isEaten = true;
                                agent.EatenResource = true;
                                agent.EatenResourceTag = "pond";
                        }
                }

                // When the agent eats food, the position of the food is changed (effectively, eating it is similar to spawning it elsewhere)
                if (isEaten)
                {
                        // myArea.ResetResourcePosition(other);
                        resourceSpawner.RelocateResource(other);

                }
        }
}
