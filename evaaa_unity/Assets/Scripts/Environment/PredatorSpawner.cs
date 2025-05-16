using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utility;

[System.Serializable]
public class PredatorGroup
{
    public string prefabName;
    public int count;
    public PositionRange position;
    public RotationRange rotationRange;
    public ScaleRange scaleRange;
    public float walkSpeed;
    public float turnSpeed;
    public float viewAngle;
    public float viewDistance;
    public float damageAmount;
    public float maxDamage;
    public float attackInterval;
    public int maxRestingSteps;
    public int maxSearchingSteps;
    public int searchingActionInterval;
    public float padding = 2.0f; // Default padding value
}

[System.Serializable]
public class PredatorConfig
{
    public List<PredatorGroup> groups;
}

public class PredatorSpawner : MonoBehaviour
{
    [Header("Configuration")]
    public string configFileName = "predatorConfig.json";
    public GameObject predatorPrefab;

    private PredatorConfig predatorConfig;
    private List<GameObject> spawnedPredators = new List<GameObject>();
    private Transform courtTransform;
    private int predatorCounter = 0;

    private ConfigLoader configLoader;

    public void InitializePredatorSpawner(ConfigLoader loader, Transform court)
    {
        configLoader = loader;
        if (configLoader == null)
        {
            Debug.LogError("ConfigLoader is not set. Ensure ConfigLoader is initialized.");
            return;
        }

        if (predatorPrefab == null)
        {
            Debug.LogError("Predator prefab is not assigned. Please assign a predator prefab to the PredatorSpawner.");
            return;
        }

        LoadConfig();

        if (predatorConfig == null)
        {
            Debug.LogError("Predator configuration is not loaded. Call ReloadConfig() before InitializePredators().");
            return;
        }

        courtTransform = court;
        predatorCounter = 0;
        GeneratePredators();
    }

    public void InitializeNavMeshForPredators()
    {
        foreach (var predator in spawnedPredators)
        {
            if (predator != null)
            {
                var predatorScript = predator.GetComponent<Predator>();
                if (predatorScript != null)
                {
                    predatorScript.InitializeNavMesh();
                }
            }
        }
        Debug.Log($"PredatorSpawner: NavMesh initialized for {spawnedPredators.Count} predators");
    }

    private void LoadConfig()
    {
        if (configLoader == null)
        {
            Debug.LogError("ConfigLoader is not set. Ensure ConfigLoader is initialized.");
            return;
        }

        predatorConfig = configLoader.LoadConfig<PredatorConfig>(configFileName);

        if (predatorConfig == null || predatorConfig.groups == null)
        {
            Debug.LogError("Invalid or empty predator configuration.");
        }
    }

    // public IEnumerator ClearAndGeneratePredators()
    // {
    //     ClearPredators();
    //     yield return new WaitForSeconds(0.5f);
    //     GeneratePredators();
    //     Debug.Log("PredatorSpawner: New predators generated.");
    // }

    public void GeneratePredators()
    {
        if (predatorConfig == null || predatorConfig.groups == null)
        {
            Debug.LogError("Predator configuration is not loaded.");
            return;
        }

        foreach (var group in predatorConfig.groups)
        {
            SpawnPredatorGroup(group);
        }
    }

    public void ClearPredators()
    {
        foreach (var predator in spawnedPredators)
        {
            if (predator != null) Destroy(predator);
        }
        spawnedPredators.Clear();
        predatorCounter = 0;
        Resources.UnloadUnusedAssets();
        Debug.Log("PredatorSpawner: Old predators have been cleared.");
    }

    private void SpawnPredatorGroup(PredatorGroup group)
    {
        if (predatorPrefab == null)
        {
            Debug.LogError("Predator prefab is not assigned.");
            return;
        }

        for (int i = 0; i < group.count; i++)
        {
            Vector3 position;
            Quaternion rotation;
            Vector3 scale;
            int attempts = 0;
            bool validPosition = false;

            do
            {
                position = RandomPosition(group.position);
                rotation = RandomRotation(group.rotationRange);
                scale = RandomScale(group.scaleRange);
                attempts++;
                validPosition = !OverlapUtility.IsOverlapping(position, predatorPrefab, scale, 1.0f, group.padding, "Landmark");
            } while (!validPosition && attempts < 100);

            if (validPosition)
            {
                predatorCounter++;
                GameObject predator = Instantiate(predatorPrefab, position, rotation);
                predator.name = $"Predator_{predatorCounter}";

                if (courtTransform != null)
                {
                    predator.transform.SetParent(courtTransform);
                }

                predator.transform.localScale = scale;

                Predator predatorScript = predator.GetComponent<Predator>();
                if (predatorScript != null)
                {
                    predatorScript.walkSpeed = group.walkSpeed;
                    predatorScript.turnSpeed = group.turnSpeed;
                    predatorScript.viewAngle = group.viewAngle;
                    predatorScript.viewDistance = group.viewDistance;
                    predatorScript.damageAmount = group.damageAmount;
                    predatorScript.maxDamage = group.maxDamage;
                    predatorScript.attackInterval = group.attackInterval;
                    predatorScript.maxRestingSteps = group.maxRestingSteps;
                    predatorScript.maxSearchingSteps = group.maxSearchingSteps;
                    predatorScript.searchingActionInterval = group.searchingActionInterval;
                    
                    predatorScript.InitializePredator();
                }

                spawnedPredators.Add(predator);
                Debug.Log($"Spawned {predator.name} at position {position}");
            }
            else
            {
                Debug.LogWarning($"Could not find a valid position for predator {predatorCounter + 1} after {attempts} attempts.");
            }
        }
    }

    private Vector3 RandomPosition(PositionRange position) =>
        new Vector3(
            Random.Range(position.xMin, position.xMax),
            Random.Range(position.yMin, position.yMax),
            Random.Range(position.zMin, position.zMax)
        );

    private Quaternion RandomRotation(RotationRange rotationRange) =>
        Quaternion.Euler(
            rotationRange.x,
            rotationRange.y,
            rotationRange.z
        );

    private Vector3 RandomScale(ScaleRange scaleRange) =>
        new Vector3(
            Random.Range(scaleRange.xMin, scaleRange.xMax),
            Random.Range(scaleRange.yMin, scaleRange.yMax),
            Random.Range(scaleRange.zMin, scaleRange.zMax)
        );

    public List<GameObject> GetSpawnedPredators()
    {
        return spawnedPredators;
    }
}
