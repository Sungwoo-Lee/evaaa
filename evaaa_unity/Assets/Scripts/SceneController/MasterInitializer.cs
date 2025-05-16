using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.MLAgents;
// using Assets.Scripts.Utility;

public class MasterInitializer : MonoBehaviour
{
    private Academy academy;
    public ConfigLoader configLoader;
    public SpawnerManager spawnerManager;
    public NavMeshSurface navMeshSurface; // Reference to the NavMeshSurface component
    public ThermoGridSpawner thermoGridSpawner;
    public InteroceptiveAgent agent; // Single agent reference
    public HeatMap heatMap;
    public DayAndNight dayAndNight;
    public AgentFollowCamera agentFollowCamera;
    // public CaptureScreenShot captureScreenShot;

    private void Start()
    {
        academy = Academy.Instance;
        InitializeScene();
    }

    private void InitializeScene()
    {
        // Step 1: Pause ML-Agents Academy
        academy.AutomaticSteppingEnabled = false;

        // Step 2: Load Configuration
        if (configLoader != null)
        {
            configLoader.InitializeConfigLoader(); // Ensure the configuration folder is set
            Debug.Log("MasterInitializer: ConfigLoader initialized.");
        }
        else
        {
            Debug.LogError("ConfigLoader is not assigned.");
            return;
        }

        // Step 3: Initialize Spawners (including predators but without NavMesh)
        if (spawnerManager != null)
        {
            spawnerManager.InitializeSpawners(configLoader);
        }
        else
        {
            Debug.LogError("SpawnerManager is not assigned.");
            return;
        }

        // Step 4: Bake NavMesh
        if (navMeshSurface != null)
        {
            Debug.Log("MasterInitializer: Baking NavMesh...");
            navMeshSurface.BuildNavMesh(); // Dynamically bake the NavMesh
            Debug.Log("MasterInitializer: NavMesh baked successfully.");
        }
        else
        {
            Debug.LogError("NavMeshSurface is not assigned. NavMesh cannot be baked.");
        }

        // Step 5: Initialize NavMesh for Predators
        if (spawnerManager != null)
        {
            spawnerManager.InitializePredatorNavMesh();
        }

        // Step 6: Initialize ThermoGridSpawner
        if (thermoGridSpawner != null)
        {
            thermoGridSpawner.InitializeThermoGridSpawner(configLoader);
            if (thermoGridSpawner == null || !thermoGridSpawner.isThermalGridReady)
            {
                Debug.LogError("ThermoGridSpawner is not ready. Initialization failed.");
                return;
            }
        }
        else
        {
            Debug.LogError("ThermoGridSpawner is not assigned.");
            return;
        }

        // Step 7: Initialize Agent
        if (agent != null)
        {
            agent.InitializeAgent(configLoader);
            Debug.Log("MasterInitializer: Agent initialized.");
        }
        else
        {
            Debug.LogError("Agent is not assigned.");
            return;
        }

        // Step 8: Initialize HeatMap
        if (heatMap != null)
        {
            heatMap.InitializeHeatMap();
            if (heatMap.isInitialized)
            {
                Debug.Log("MasterInitializer: HeatMap initialized.");
            }
            else
            {
                Debug.LogError("MasterInitializer: HeatMap failed to initialize.");
            }
            heatMap.EpisodeHeatMap();
        }
        else
        {
            Debug.LogError("HeatMap is not assigned.");
            return;
        }

        // Step 9: Initialize Day/Night Cycle
        if (dayAndNight != null)
        {
            dayAndNight.InitializeDayAndNight(configLoader);
            Debug.Log("MasterInitializer: DayAndNight initialized.");

            // Set DayAndNight reference on all predators
            var predators = FindObjectsOfType<Predator>();
            foreach (var predator in predators)
            {
                predator.SetDayAndNight(dayAndNight);
            }
            Debug.Log($"MasterInitializer: Set DayAndNight reference on {predators.Length} predators.");
        }
        else
        {
            Debug.LogError("DayAndNight is not assigned.");
            return;
        }

        // Step 10: Initialize Camera
        if (agentFollowCamera != null)
        {
            agentFollowCamera.InitializeCamera(configLoader);
            Debug.Log("MasterInitializer: AgentFollowCamera initialized.");
        }
        else
        {
            Debug.LogError("AgentFollowCamera is not assigned.");
            return;
        }

        // Step 11: Initialize Systems
        InitializeSystems();

        // Step 12: Resume ML-Agents Academy
        InteroceptiveAgent.isEnvironmentReady = true;
        academy.AutomaticSteppingEnabled = true;
        Debug.Log("MasterInitializer: Scene fully initialized, ML-Agents enabled.");
    }

    private void InitializeSystems()
    {
        // Initialize the experiment system
        DataRecorder dataRecorder = FindObjectOfType<DataRecorder>();
        if (dataRecorder != null)
        {
            dataRecorder.Initialize(agent);
        }
        else
        {
            Debug.LogWarning("dataRecorder not found in the scene. Experiment metrics will not be recorded.");
        }

        // Initialize the event system
        EventManager eventManager = FindObjectOfType<EventManager>();
        if (eventManager != null)
        {
            eventManager.InitializeEventManager(configLoader);
            Debug.Log("MasterInitializer: EventManager initialized.");
        }
        else
        {
            Debug.LogWarning("EventManager not found in the scene. Event system will not be available.");
        }

        CaptureScreenShot captureScreenShot = FindObjectOfType<CaptureScreenShot>();
        // Initialize screen capture system
        if (captureScreenShot != null)
        {
            captureScreenShot.Initialize();
            Debug.Log("MasterInitializer: CaptureScreenShot initialized.");
        }
        else
        {
            Debug.LogError("CaptureScreenShot is not assigned.");
        }

        // Initialize other systems...
    }

    public void ResetScene()
    {
        StartCoroutine(ResetSceneInOrder());
    }

    private IEnumerator ResetSceneInOrder()
    {
        Debug.Log("MasterInitializer: Resetting scene...");
        // Step 1: Pause the Academy/Agent updates during reset
        academy.AutomaticSteppingEnabled = false;

        // Step 2: Reset random spawners and predators through SpawnerManager
        if (spawnerManager != null)
        {
            // yield return StartCoroutine(spawnerManager.ResetAllSpawnersCoroutine());
            spawnerManager.ResetAllSpawners();
            Debug.Log("MasterInitializer: Random obstacles and resources reset.");
        }
        else
        {
            Debug.LogError("SpawnerManager is not assigned.");
        }

        // Step 3: Bake the NavMesh again (necessary for updated obstacle positions)
        if (navMeshSurface != null)
        {
            Debug.Log("MasterInitializer: Baking NavMesh after reset...");
            navMeshSurface.BuildNavMesh();
            Debug.Log("MasterInitializer: NavMesh baked successfully after reset.");
            yield return null;
        }
        else
        {
            Debug.LogError("NavMeshSurface is not assigned. NavMesh cannot be baked after reset.");
        }

        // Step 4: Initialize predator NavMesh
        if (spawnerManager != null)
        {
            spawnerManager.InitializePredatorNavMesh();
        }

        // Step 5: Reset DayAndNight
        if (dayAndNight != null)
        {
            dayAndNight.ResetDayAndNight();
            Debug.Log("MasterInitializer: DayAndNight reset.");
            
            // Update DayAndNight reference on all predators
            var predators = FindObjectsOfType<Predator>();
            foreach (var predator in predators)
            {
                predator.SetDayAndNight(dayAndNight);
            }
            Debug.Log($"MasterInitializer: Updated DayAndNight reference on {predators.Length} predators.");
        }
        else
        {
            Debug.LogError("DayAndNight is not assigned.");
        }

        // Step 6: Update HeatMap
        if (heatMap != null)
        {
            heatMap.EpisodeHeatMap();
            Debug.Log("MasterInitializer: HeatMap reset.");
        }
        else
        {
            Debug.LogError("HeatMap is not assigned.");
        }

        // Step 7: Reset Event System
        EventManager eventManager = FindObjectOfType<EventManager>();
        if (eventManager != null)
        {
            eventManager.ResetEventManager();
            Debug.Log("MasterInitializer: Event system reset.");
        }

        // Step 8: Resume the Academy/Agent updates
        academy.AutomaticSteppingEnabled = true;
        Debug.Log("MasterInitializer: Scene reset complete, ML-Agents enabled.");
    }

    private bool IsHeatMapReady()
    {
        var thermoGridSpawner = FindObjectOfType<ThermoGridSpawner>();
        return thermoGridSpawner != null && heatMap != null && heatMap.enabled;
    }
}