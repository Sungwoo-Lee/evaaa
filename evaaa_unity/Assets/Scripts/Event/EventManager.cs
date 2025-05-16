using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EventGroup
{
    public string name;
    public string triggerTag;
    public string targetTag;
    public int maxCount;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public float foodValue;
    public float waterValue;
    public string message;
}

[System.Serializable]
public class EventConfig
{
    public List<EventGroup> groups;
}

public class EventManager : MonoBehaviour
{
    [Header("Configuration")]
    public string configFileName = "eventConfig.json";
    private EventConfig config;
    private List<GameObject> spawnedTriggers = new List<GameObject>();
    private ConfigLoader configLoader;
    private int messageCount = 0;
    private int resourceCount = 0;

    public void InitializeEventManager(ConfigLoader loader)
    {
        configLoader = loader;
        if (configLoader == null)
        {
            Debug.LogError("ConfigLoader is not assigned.");
            return;
        }

        // Load the event configuration
        config = configLoader.LoadConfig<EventConfig>(configFileName);
        if (config == null)
        {
            Debug.LogError("Failed to load event configuration!");
            return;
        }

        // Generate triggers
        GenerateTriggers();

        // Register event handlers
        RegisterEventHandlers();

        Debug.Log("EventManager: Initialized successfully.");
    }

    private void GenerateTriggers()
    {
        if (config == null || config.groups == null)
        {
            Debug.LogError("Event configuration is not valid.");
            return;
        }

        // Clear any existing triggers
        ClearTriggers();

        // Generate new triggers
        foreach (var group in config.groups)
        {
            SpawnTrigger(group);
        }

        Debug.Log($"EventManager: Generated {config.groups.Count} triggers.");
    }

    private void SpawnTrigger(EventGroup group)
    {
        // Create a new GameObject for the trigger
        GameObject trigger = new GameObject(group.name);
        
        // Set transform properties
        trigger.transform.position = group.position;
        trigger.transform.rotation = Quaternion.Euler(group.rotation);
        trigger.transform.localScale = group.scale;

        // Add and configure the BoxCollider
        BoxCollider collider = trigger.AddComponent<BoxCollider>();
        collider.isTrigger = true;

        // Add and configure the TriggerZone component
        GameEventSystem.TriggerZone triggerZone = trigger.AddComponent<GameEventSystem.TriggerZone>();
        triggerZone.SetTriggerTag(group.triggerTag);
        triggerZone.SetTargetTag(group.targetTag);

        spawnedTriggers.Add(trigger);
    }

    private void ClearTriggers()
    {
        foreach (var trigger in spawnedTriggers)
        {
            if (trigger != null) Destroy(trigger);
        }
        spawnedTriggers.Clear();
        Debug.Log("EventManager: Cleared all triggers.");
    }

    private void RegisterEventHandlers()
    {
        foreach (var group in config.groups)
        {
            if (!string.IsNullOrEmpty(group.message))
            {
                GameEventSystem.Register(group.triggerTag, HandleMessageEvent, group.maxCount);
            }
            else
            {
                GameEventSystem.Register(group.triggerTag, HandleResourceEvent, group.maxCount);
            }
        }
        Debug.Log($"EventManager: Registered {config.groups.Count} event handlers.");
    }

    // Explicit method for handling message events
    private void HandleMessageEvent(GameObject obj)
    {
        var group = config.groups.Find(g => g.triggerTag == "message");
        if (group != null)
        {
            Debug.Log($"[{obj.name}]: {group.message}");
            
            // Record the event in metrics
            var agent = obj.GetComponent<InteroceptiveAgent>();
            if (agent != null && agent.dataRecorder != null)
            {
                agent.dataRecorder.RecordEvent("message");
            }
        }
    }

    // Explicit method for handling resource events
    private void HandleResourceEvent(GameObject obj)
    {
        var group = config.groups.Find(g => g.triggerTag == "resource");
        if (group != null)
        {
            var agent = obj.GetComponent<InteroceptiveAgent>();
            if (agent != null)
            {
                agent.resourceLevels[0] = Mathf.Clamp(group.foodValue, agent.foodLevelRange.min, agent.foodLevelRange.max);
                agent.resourceLevels[1] = Mathf.Clamp(group.waterValue, agent.waterLevelRange.min, agent.waterLevelRange.max);
                Debug.Log($"[Resource] Set {obj.name}: Food={group.foodValue}, Water={group.waterValue}");
                
                // Record the event in metrics
                if (agent.dataRecorder != null)
                {
                    agent.dataRecorder.RecordEvent("resource");
                }
            }
        }
    }

    void FixedUpdate()
    {
        // Reset counts on R key
        if (Input.GetKeyDown(KeyCode.R) && config != null)
        {
            foreach (var group in config.groups)
            {
                GameEventSystem.ResetCount(group.triggerTag);
            }
            Debug.Log("EventManager: Reset all event counts.");
        }
    }

    private void OnDestroy()
    {
        ClearTriggers();
    }

    public void ResetEventManager()
    {
        // Reset event counts
        messageCount = 0;
        resourceCount = 0;

        // Clear all event handlers
        GameEventSystem.ClearAllEventHandlers();

        // Reinitialize with current configuration
        if (configLoader != null)
        {
            InitializeEventManager(configLoader);
            Debug.Log("EventManager: Reset and reinitialized with current configuration.");
        }
        else
        {
            Debug.LogWarning("EventManager: Cannot reset - ConfigLoader is null.");
        }
    }
} 