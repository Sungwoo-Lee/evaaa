using System;
using UnityEngine;
// using MathNet.Numerics;

// Used when declaring the prefab member variable of the Food class
public class ResourceProperty : MonoBehaviour
{
        int VectorSize = 10;
        public float[] property = { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
        private float[] FoodProperty = { 1f, 1f, 1f, 1f, 1f, 0f, 0f, 0f, 0f, 0f };
        private float[] WaterProperty = { 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 1f, 1f };

        public float[] ResourceP { get; private set; }

        // Function to initialize the property of food (initialize vector components)
        public void InitializeProperties()
        {
                if (gameObject.CompareTag("food"))
                {
                        // ResourceP = AddNoise(FoodProperty);
                        ResourceP = AddNoise(property);
                }
                else if (gameObject.CompareTag("water"))
                {
                        // ResourceP = AddNoise(WaterProperty);
                        ResourceP = AddNoise(property);
                }
                else if (gameObject.CompareTag("pond"))
                {
                        // ResourceP = AddNoise(WaterProperty);
                        ResourceP = AddNoise(property);
                }

        }

        // Function to set noise for food when sniffing
        private float[] AddNoise(float[] property)
        {
                for (int i = 0; i < VectorSize; i++)
                {
                        //easy
                        property[i] += 0f;

                        // Gaussian
                        // float noise = (float)Generate.Normal(1, 0, 0.1)[0];
                        // property[i] += noise;

                        // Uniform(0~1)
                        //System.Random r = new System.Random();
                        //double noise = r.NextDouble();
                        //property[i] += (float)noise;
                }
                return property;
        }
}
