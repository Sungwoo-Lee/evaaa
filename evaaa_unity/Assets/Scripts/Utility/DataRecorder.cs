using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class DataRecorder : MonoBehaviour
{
    [Header("Experiment Configuration")]
    public string experimentType;
    public int episodeNumber;
    public bool isActive = true;
    public InteroceptiveAgent targetAgent;
    private bool recordEnable;

    [Header("Step-level Metrics")]
    public List<StepData> stepData = new List<StepData>();
    private string stepDataFileName;

    [Header("Episode-level Metrics")]
    public List<EpisodeData> episodeData = new List<EpisodeData>();
    private string episodeDataFileName;
    public EpisodeData currentEpisode;

    [Header("Data Export")]
    private string outputDirectory;
    private string baseFolderName;
    private string fileNamePrefix;
    private MainConfig mainConfig;

    // Track event data
    private HashSet<string> eventTypesInEpisode = new HashSet<string>();
    private int totalEventsInEpisode = 0;

    // --- Merged ExperimentManager logic ---
    private string currentAction;  // Store the current action
    public string resourceOffered = "";
    // --- End merged ExperimentManager logic ---

    private int globalStepNumber = 0; // Tracks total steps across all episodes
    private int episodeStepNumber = 0; // Tracks steps within the current episode
    private int episodeStepStartIndex = 0; // Tracks where the current episode's steps start in stepData

    public class StepData
    {
        public int stepNumber;
        public float foodLevel;
        public float waterLevel;
        public float thermoLevel;
        public float healthLevel;
        public Vector3 position;
        public string action;
        public float reward;
        public float distanceTraveled;
        public bool isEpisodeEnd;
        public bool hasCollision;
        public bool resourceConsumed;
        public string consumedResourceType;
        // New event-related fields
        public bool hasEvent;
        public string eventType;
    }

    public class EpisodeData
    {
        public int episodeNumber;
        public int totalSteps;
        public float averageReward;
        public float maxReward;
        public float minReward;
        public int foodConsumed;
        public int waterConsumed;
        public float finalFoodLevel;
        public float finalWaterLevel;
        public float finalHealthLevel;
        public float finalTempLevel;
        public int collisions;
        public Dictionary<string, float> actionPercentages;
        public string episodeEndType;
        // New event-related fields
        public int totalEvents;
        public int uniqueEventTypes;
    }

    public void Initialize(InteroceptiveAgent agent)
    {
        targetAgent = agent;
        if (targetAgent == null)
        {
            Debug.LogError("DataRecorder: Target agent not assigned!");
            isActive = false;
            return;
        }

        // Get ConfigLoader and mainConfig
        ConfigLoader configLoader = FindObjectOfType<ConfigLoader>();
        if (configLoader != null)
        {
            mainConfig = configLoader.mainConfig;
            if (mainConfig != null)
            {
                baseFolderName = mainConfig.experimentData.baseFolderName;
                fileNamePrefix = mainConfig.experimentData.fileNamePrefix;
                recordEnable = mainConfig.experimentData.recordEnable;
            }
            else
            {
                Debug.LogError("DataRecorder: mainConfig or experimentData is null");
                isActive = false;
                return;
            }
        }
        else
        {
            Debug.LogError("DataRecorder: ConfigLoader not found");
            isActive = false;
            return;
        }

        if (recordEnable)
        {
            // Create timestamp-based directory structure
            string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");

#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
            string appRoot = Directory.GetParent(Application.dataPath).Parent.FullName;
            outputDirectory = Path.Combine(appRoot, baseFolderName);
#else
            outputDirectory = Path.Combine(Application.dataPath, "..",  baseFolderName);
#endif
            
            Debug.Log($"DataRecorder: Using outputDirectory={outputDirectory}");
            
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            InitializeDataFiles();
        }
        isActive = true;
        episodeNumber = 0; // Initialize episode number from 0
    }

    public void InitializeEpisode()
    {
        currentEpisode = new EpisodeData
        {
            episodeNumber = this.episodeNumber,
            totalSteps = 0,
            averageReward = 0f,
            maxReward = float.MinValue,
            minReward = float.MaxValue,
            foodConsumed = 0,
            waterConsumed = 0,
            finalFoodLevel = 0f,
            finalWaterLevel = 0f,
            finalHealthLevel = 0f,
            finalTempLevel = 0f,
            collisions = 0,
            actionPercentages = new Dictionary<string, float>(),
            episodeEndType = "Unknown",
            totalEvents = 0,
            uniqueEventTypes = 0
        };
        episodeStepNumber = 0; // Reset per-episode step counter
        episodeStepStartIndex = stepData.Count; // Mark where this episode's steps start
    }

    private void InitializeDataFiles()
    {
        if (!recordEnable) return;

        // Generate filenames with timestamp
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        stepDataFileName = Path.Combine(outputDirectory, $"{fileNamePrefix}{experimentType}_steps_{timestamp}.csv");
        episodeDataFileName = Path.Combine(outputDirectory, $"{fileNamePrefix}{experimentType}_episodes_{timestamp}.csv");

        // Initialize step data file header
        using (StreamWriter writer = new StreamWriter(stepDataFileName))
        {
            writer.WriteLine("Episode," +
                "GlobalStep," +
                "EpisodeStep," +
                "FoodLevel," +
                "WaterLevel," +
                "ThermoLevel," +
                "HealthLevel," +
                "PositionX," +
                "PositionY," +
                "PositionZ," +
                "Action," +
                "Reward," +
                "DistanceTraveled," +
                "IsEpisodeEnd," +
                "HasCollision," +
                "ResourceConsumed," +
                "ConsumedResourceType," +
                "HasEvent," +
                "EventType");
        }

        // Initialize episode data file header
        using (StreamWriter writer = new StreamWriter(episodeDataFileName))
        {
            writer.WriteLine("Episode," +
                "TotalSteps," +
                "AvgReward," +
                "MaxReward," +
                "MinReward," +
                "FoodConsumed," +
                "WaterConsumed," +
                "FinalFoodLevel," +
                "FinalWaterLevel," +
                "FinalTempLevel," +
                "FinalHealthLevel," +
                "Collisions," +
                "None%," +
                "Forward%," +
                "Left%," +
                "Right%," +
                "Eat%," +
                "EpisodeEndType," +
                "TotalEvents," +
                "UniqueEventTypes");
        }

        // Initialize current episode data
        currentEpisode = new EpisodeData
        {
            episodeNumber = this.episodeNumber,
            totalSteps = 0,
            averageReward = 0f,
            maxReward = float.MinValue,
            minReward = float.MaxValue,
            foodConsumed = 0,
            waterConsumed = 0,
            finalFoodLevel = 0f,
            finalWaterLevel = 0f,
            finalHealthLevel = 0f,
            finalTempLevel = 0f,
            collisions = 0,
            actionPercentages = new Dictionary<string, float>(),
            episodeEndType = "Unknown",
            totalEvents = 0,
            uniqueEventTypes = 0
        };
    }

    private void RecordStepInternal(string action, bool isFinalStep, bool hasEvent = false, string eventType = "")
    {
        if (!isActive || targetAgent == null) return;

        // Ensure currentEpisode is initialized
        if (currentEpisode == null)
        {
            InitializeEpisode();
        }

        globalStepNumber++; // Increment global step counter
        episodeStepNumber++; // Increment per-episode step counter

        // Create new step data
        StepData step = new StepData
        {
            stepNumber = globalStepNumber, // For backward compatibility, keep this as global step
            foodLevel = targetAgent.resourceLevels[0],
            waterLevel = targetAgent.resourceLevels[1],
            thermoLevel = targetAgent.resourceLevels[2],
            healthLevel = targetAgent.resourceLevels[3],
            position = targetAgent.transform.position,
            reward = targetAgent.currentReward,
            distanceTraveled = Vector3.Distance(targetAgent.transform.position, 
                stepData.Count > 0 ? stepData[stepData.Count - 1].position : targetAgent.transform.position),
            action = action,
            isEpisodeEnd = isFinalStep,
            hasCollision = targetAgent.countCollision > 0,
            resourceConsumed = targetAgent.resourceConsumedInStep,
            consumedResourceType = targetAgent.consumedResourceType,
            hasEvent = hasEvent,
            eventType = eventType
        };

        // Add to step data list
        stepData.Add(step);

        // Update episode step count
        currentEpisode.totalSteps++;

        if (recordEnable)
        {
            // Write to step data file immediately
            try
            {
                using (StreamWriter writer = new StreamWriter(stepDataFileName, true))
                {
                    writer.WriteLine($"{episodeNumber}," +
                        $"{globalStepNumber}," +
                        $"{episodeStepNumber}," +
                        $"{step.foodLevel:F2}," +
                        $"{step.waterLevel:F2}," +
                        $"{step.thermoLevel:F2}," +
                        $"{step.healthLevel:F2}," +
                        $"{step.position.x:F2}," +
                        $"{step.position.y:F2}," +
                        $"{step.position.z:F2}," +
                        $"{step.action}," +
                        $"{step.reward:F2}," +
                        $"{step.distanceTraveled:F2}," +
                        $"{step.isEpisodeEnd}," +
                        $"{step.hasCollision}," +
                        $"{step.resourceConsumed}," +
                        $"{step.consumedResourceType}," +
                        $"{step.hasEvent}," +
                        $"{step.eventType}");
                }

                // Debug log for events
                if (hasEvent)
                {
                    Debug.Log($"DataRecorder: Recorded event '{eventType}' at step {globalStepNumber}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error writing step data: {e.Message}");
            }
        }
    }

    // --- Merged ExperimentManager logic (public API) ---
    public void RecordAction(string action)
    {
        if (!isActive) return;
        // Track current action for step-level recording
        currentAction = action;
        // Update action counts for current episode
        if (currentEpisode == null) InitializeEpisode();
        if (!currentEpisode.actionPercentages.ContainsKey(action))
            currentEpisode.actionPercentages[action] = 0;
        currentEpisode.actionPercentages[action]++;
        // Update last step's action
        if (stepData.Count > 0)
            stepData[stepData.Count - 1].action = action;
    }

    public void RecordStep()
    {
        if (!isActive) return;
        RecordStepInternal(currentAction, false);
    }

    public void RecordStep(string action)
    {
        RecordStepInternal(action, false);
    }

    public void RecordFinalStep()
    {
        if (!isActive) return;
        RecordStepInternal(currentAction, true);
    }

    public void RecordFinalStep(string action)
    {
        RecordStepInternal(action, true);
    }

    public void RecordCollision()
    {
        if (!isActive) return;
        if (currentEpisode == null) InitializeEpisode();
        currentEpisode.collisions++;
    }

    public void RecordFoodConsumed()
    {
        if (!isActive) return;
        if (currentEpisode == null) InitializeEpisode();
        currentEpisode.foodConsumed++;
    }

    public void RecordWaterConsumed()
    {
        if (!isActive) return;
        if (currentEpisode == null) InitializeEpisode();
        currentEpisode.waterConsumed++;
    }

    public void RecordEvent(string eventType)
    {
        if (!isActive) return;
        // Record the event in the current step
        RecordStepInternal("", false, true, eventType);
        // Update episode-level event tracking
        totalEventsInEpisode++;
        eventTypesInEpisode.Add(eventType);
        // Update current episode data immediately
        if (currentEpisode != null)
        {
            currentEpisode.totalEvents = totalEventsInEpisode;
            currentEpisode.uniqueEventTypes = eventTypesInEpisode.Count;
        }
        Debug.Log($"DataRecorder: Recorded event '{eventType}'");
    }

    public void ExportEpisodeSummary()
    {
        if (!isActive || !recordEnable) return;
        // Update episode data with event information
        currentEpisode.totalEvents = totalEventsInEpisode;
        currentEpisode.uniqueEventTypes = eventTypesInEpisode.Count;
        // Write to episode data file
        try
        {
            using (StreamWriter writer = new StreamWriter(episodeDataFileName, true))
            {
                writer.WriteLine($"{currentEpisode.episodeNumber}," +
                    $"{currentEpisode.totalSteps}," +
                    $"{currentEpisode.averageReward:F2}," +
                    $"{currentEpisode.maxReward:F2}," +
                    $"{currentEpisode.minReward:F2}," +
                    $"{currentEpisode.foodConsumed}," +
                    $"{currentEpisode.waterConsumed}," +
                    $"{currentEpisode.finalFoodLevel:F2}," +
                    $"{currentEpisode.finalWaterLevel:F2}," +
                    $"{currentEpisode.finalTempLevel:F2}," +
                    $"{currentEpisode.finalHealthLevel:F2}," +
                    $"{currentEpisode.collisions}," +
                    $"{GetActionPercentage("None"):F2}," +
                    $"{GetActionPercentage("Forward"):F2}," +
                    $"{GetActionPercentage("Left"):F2}," +
                    $"{GetActionPercentage("Right"):F2}," +
                    $"{GetActionPercentage("Eat"):F2}," +
                    $"{currentEpisode.episodeEndType}," +
                    $"{currentEpisode.totalEvents}," +
                    $"{currentEpisode.uniqueEventTypes}");
                Debug.Log($"DataRecorder: Exported episode {currentEpisode.episodeNumber} summary with {currentEpisode.totalEvents} total events and {currentEpisode.uniqueEventTypes} unique event types.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error writing episode data: {e.Message}");
        }
        // Reset event tracking for next episode
        totalEventsInEpisode = 0;
        eventTypesInEpisode.Clear();
    }

    public void CalculateFinalMetrics()
    {
        // Only use steps from this episode
        var episodeSteps = stepData.Skip(episodeStepStartIndex).ToList();
        if (episodeSteps.Count == 0) return;
        currentEpisode.totalSteps = episodeSteps.Count;
        // Calculate reward statistics
        currentEpisode.averageReward = episodeSteps.Average(s => s.reward);
        currentEpisode.maxReward = episodeSteps.Max(s => s.reward);
        currentEpisode.minReward = episodeSteps.Min(s => s.reward);
        // Get final resource levels
        currentEpisode.finalFoodLevel = episodeSteps.Last().foodLevel;
        currentEpisode.finalWaterLevel = episodeSteps.Last().waterLevel;
        currentEpisode.finalHealthLevel = episodeSteps.Last().healthLevel;
        currentEpisode.finalTempLevel = episodeSteps.Last().thermoLevel;
        // Calculate action percentages
        float totalActions = currentEpisode.actionPercentages.Values.Sum();
        if (totalActions > 0)
        {
            foreach (var action in currentEpisode.actionPercentages.Keys.ToList())
            {
                currentEpisode.actionPercentages[action] = (currentEpisode.actionPercentages[action] / totalActions) * 100f;
            }
        }
        // Ensure all actions are represented
        string[] allActions = { "None", "Forward", "Backward", "Left", "Right", "Eat", "Drink" };
        foreach (var action in allActions)
        {
            if (!currentEpisode.actionPercentages.ContainsKey(action))
            {
                currentEpisode.actionPercentages[action] = 0f;
            }
        }
    }

    private float GetActionPercentage(string action)
    {
        return currentEpisode.actionPercentages.ContainsKey(action) ? currentEpisode.actionPercentages[action] : 0f;
    }

//     public void ResetMetrics()
//     {
//         stepData.Clear();
//         episodeData.Clear();
//         currentEpisode = null;
//         totalEventsInEpisode = 0;
//         eventTypesInEpisode.Clear();
//         episodeNumber = 1;
//     }

//     public void ResetExperiment()
//     {
//         ExportEpisodeSummary();
//         ResetMetrics();
//     }

    public void SetResourceOffered(string resource)
    {
        resourceOffered = resource;
    }

    public void OnEpisodeBegin()
    {
        // Call this ONLY at the start of a new episode, after the previous episode has ended and been exported.
        episodeNumber++;
        // this.episodeNumber = episodeNumber;
        InitializeEpisode();
    }

    public void RecordResourceChoice(string resourceChosen)
    {
        RecordStep();
    }

    public void OnEpisodeEnd()
    {
        // Call this ONLY at the end of an episode, before starting a new one.
        RecordFinalStep();
        CalculateFinalMetrics();
        // RecordFinalStep();
        ExportEpisodeSummary();
    }

    public void SetEpisodeEndType(string endType)
    {
        if (!isActive) return;
        if (currentEpisode == null)
        {
            InitializeEpisode();
        }
        currentEpisode.episodeEndType = endType;
    }

    [System.Serializable]
    private class ConfigData
    {
        public ExperimentDataConfig experimentData;
    }

    [System.Serializable]
    private class ExperimentDataConfig
    {
        public string baseFolderName;
        public string fileNamePrefix;
        public bool recordEnable;
    }
} 