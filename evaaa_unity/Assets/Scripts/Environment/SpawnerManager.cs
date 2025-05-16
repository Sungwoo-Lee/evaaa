using UnityEngine;
using System.Collections;

public class SpawnerManager : MonoBehaviour
{
    [Header("Spawner References")]
    public CourtSpawner courtSpawner;
    public PredatorSpawner predatorSpawner;
    public ObstacleSpawner obstacleSpawner;
    public ResourceSpawner resourceSpawner;
    public ThermoGridSpawner thermoGridSpawner;
    [Header("Landmark Settings")]
    public LandmarkSpawner landmarkSpawner;

    private bool hasPredators = false;

    public void InitializeSpawners(ConfigLoader configLoader)
    {
        Debug.Log("SpawnerManager: Initializing spawners...");
        
        // Step 1: Initialize CourtSpawner (REQUIRED)
        if (courtSpawner != null)
        {
            courtSpawner.InitializeCourt(configLoader);
            Debug.Log("SpawnerManager: CourtSpawner Initialized.");
        }
        else
        {
            Debug.LogError("SpawnerManager: CourtSpawner is not assigned.");
            return;
        }

        // Step 2: Check if we have predators to spawn
        if (predatorSpawner != null)
        {
            var predatorConfig = configLoader.LoadConfig<PredatorConfig>(predatorSpawner.configFileName);
            if (predatorConfig != null && predatorConfig.groups != null)
            {
                foreach (var group in predatorConfig.groups)
                {
                    if (group != null && group.count > 0)
                    {
                        hasPredators = true;
                        break;
                    }
                }
            }
        }

        // Step 3: Initialize Predators (if present)
        if (hasPredators)
        {
            Debug.Log("SpawnerManager: Initializing predators (without NavMesh)...");
            if (predatorSpawner != null)
            {
                predatorSpawner.InitializePredatorSpawner(configLoader, courtSpawner.CourtTransform);
                Debug.Log("SpawnerManager: Predators initialized (NavMesh pending).");
            }
            else
            {
                Debug.LogError("SpawnerManager: PredatorSpawner is not assigned but predators are configured.");
                return;
            }
        }

        // Step 4: Initialize static resources and obstacles
        Debug.Log("SpawnerManager: Initializing static resources and obstacles...");
        if (resourceSpawner != null)
        {
            resourceSpawner.InitializeResourceSpawner(configLoader, onlyStatic:true);
            resourceSpawner.InitializeResources(courtSpawner.CourtTransform, onlyStatic:true);
            Debug.Log("SpawnerManager: Static resources initialized.");
        }

        if (obstacleSpawner != null)
        {
            obstacleSpawner.InitializeObstacleSpawner(configLoader, courtSpawner.CourtTransform, onlyStatic:true);
            Debug.Log("SpawnerManager: Static obstacles initialized.");
        }

        // Step 5: Generate Landmarks (after static objects to handle overlapping)
        if (hasPredators)
        {
            Debug.Log("SpawnerManager: Generating landmarks for predators...");
            if (landmarkSpawner != null && courtSpawner.CourtTransform != null)
            {
                landmarkSpawner.transform.parent = courtSpawner.CourtTransform;
                landmarkSpawner.SetCourtSpawner(courtSpawner);
                landmarkSpawner.InitializeLandmarkSpawner(configLoader, courtSpawner.CourtTransform);
                Debug.Log("SpawnerManager: Landmarks generated for predators.");
            }
            else
            {
                Debug.LogError("SpawnerManager: LandmarkSpawner is not assigned but predators are present.");
                return;
            }
        }

        // Step 6: Initialize random resources and obstacles
        Debug.Log("SpawnerManager: Initializing random resources and obstacles...");
        if (obstacleSpawner != null)
        {
            obstacleSpawner.InitializeObstacleSpawner(configLoader, courtSpawner.CourtTransform, onlyStatic:false);
            Debug.Log("SpawnerManager: Random obstacles initialized.");
        }

        if (resourceSpawner != null)
        {
            resourceSpawner.InitializeResourceSpawner(configLoader, onlyStatic:false);
            resourceSpawner.InitializeResources(courtSpawner.CourtTransform, onlyStatic:false);
            Debug.Log("SpawnerManager: Random resources initialized.");
        }

        // Step 7: Initialize ThermoGridSpawner
        Debug.Log("SpawnerManager: Initializing thermal grid...");
        if (thermoGridSpawner != null)
        {
            thermoGridSpawner.InitializeThermoGridSpawner(configLoader);
            thermoGridSpawner.InitializeGrid(courtSpawner.CourtTransform);
            Debug.Log("SpawnerManager: ThermoGridSpawner initialized.");
        }
        else
        {
            Debug.LogError("SpawnerManager: ThermoGridSpawner is not assigned.");
        }
    }

    public void InitializePredatorNavMesh()
    {
        if (hasPredators && predatorSpawner != null)
        {
            predatorSpawner.InitializeNavMeshForPredators();
            Debug.Log("SpawnerManager: NavMesh initialized for predators.");
        }
    }

    public void ResetAllSpawners()
    {
        StartCoroutine(ResetAllSpawnersCoroutine());
    }

    public IEnumerator ResetAllSpawnersCoroutine()
    {
        // Debug.Log("SpawnerManager: Starting reset sequence...");

        // Step 0: Clear all resources and obstacles
        // if (resourceSpawner != null)
        // {
            // resourceSpawner.ClearAllResources();
        // }

        if (obstacleSpawner != null)
        {
            obstacleSpawner.ClearRandomObstacles();
        }

        if (predatorSpawner != null)
        {
            predatorSpawner.ClearPredators();
        }
        

        // Step 1: Initialize predators (without NavMesh, matching initialization order)
        if (hasPredators && predatorSpawner != null)
        {
            // First clear and regenerate predators
            // predatorSpawner.ClearPredators();
            predatorSpawner.GeneratePredators();
            Debug.Log("SpawnerManager: Predators regenerated (NavMesh pending).");
        }

        // Step 2: Reset random obstacles (static obstacles remain unchanged)
        if (obstacleSpawner != null)
        {
            // yield return StartCoroutine(obstacleSpawner.ClearAndGenerateObstacles());
            // obstacleSpawner.ClearAllObstacles();
            obstacleSpawner.GenerateObstacles(onlyStatic: false);
            Debug.Log("SpawnerManager: Random obstacles reset.");
        }

        // Step 3: Reset random resources (static resources remain unchanged)
        if (resourceSpawner != null)
        {
            resourceSpawner.ResetResources();
            yield return null;
            Debug.Log("SpawnerManager: Random resources reset.");
        }

        // Step 4: Reset ThermoGridSpawner (to update thermal grid based on new obstacle positions)
        if (thermoGridSpawner != null)
        {
            thermoGridSpawner.ResetGrid();
            yield return null;
            Debug.Log("SpawnerManager: Thermal grid reset.");
        }

        Debug.Log("SpawnerManager: Reset sequence completed.");
    }
}
