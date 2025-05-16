using UnityEngine;
using System.Collections.Generic;

public static class GameEventSystem
{
    // Event storage
    private static Dictionary<string, List<System.Action<GameObject>>> events = new();
    private static Dictionary<string, int> counts = new();
    private static Dictionary<string, int> maxCounts = new();

    // Integrated TriggerZone as a public static class
    public class TriggerZone : MonoBehaviour
    {
        private string triggerTag;
        private string targetTag;

        public void SetTriggerTag(string tag)
        {
            triggerTag = tag;
        }

        public void SetTargetTag(string tag)
        {
            targetTag = tag;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(targetTag))
            {
                Trigger(triggerTag, other.gameObject);
            }
        }
    }

    // Register an event with optional max count (-1 for unlimited)
    public static void Register(string triggerTag, System.Action<GameObject> action, int maxCount = -1)
    {
        if (!events.ContainsKey(triggerTag))
        {
            events[triggerTag] = new List<System.Action<GameObject>>();
            counts[triggerTag] = 0;
            maxCounts[triggerTag] = maxCount;
        }
        events[triggerTag].Add(action);
    }

    // Trigger all events for a tag
    public static void Trigger(string triggerTag, GameObject invoker)
    {
        if (!events.ContainsKey(triggerTag)) return;
        
        // Check max count
        if (maxCounts[triggerTag] >= 0 && counts[triggerTag] >= maxCounts[triggerTag]) return;
        
        // Execute all events
        foreach (var action in events[triggerTag])
        {
            action(invoker);
        }
        
        counts[triggerTag]++;
    }

    // Reset count for a tag
    public static void ResetCount(string triggerTag)
    {
        if (counts.ContainsKey(triggerTag))
            counts[triggerTag] = 0;
    }

    // Get current count for a tag
    public static int GetCount(string triggerTag)
    {
        return counts.TryGetValue(triggerTag, out int count) ? count : 0;
    }

    public static void ClearAllEventHandlers()
    {
        events.Clear();
        counts.Clear();
        maxCounts.Clear();
        Debug.Log("GameEventSystem: All event handlers cleared.");
    }
} 