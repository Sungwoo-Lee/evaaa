using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class ThermoGridConfig
{
    public int numberOfGridCubeX;
    public int numberOfGridCubeZ;
    public float fieldDefaultTemp;
    public float hotSpotTemp;
    public int hotSpotCount;
    public float hotSpotSize;
    public float smoothingSigma;
    public bool useObjectHotSpot;
    public bool useRandomHotSpot;
    public float gridCubeHeight;
}

public class ThermoGridSpawner : MonoBehaviour
{
    public bool isThermalGridReady { get; private set; } = false;

    [Header("Thermal Grid Configuration")]
    public string configFileName = "thermoGridConfig.json";

    private ThermoGridConfig config = new ThermoGridConfig();
    private GameObject thermalGridParent;
    private float[,] areaTemp;
    private float[,] baseTemp; // Store the base temperature for each cell

    private Vector3 floorSize;
    private Vector3 floorPosition;

    private ConfigLoader configLoader; // Reference to ConfigLoader

    public int NumberOfGridCubeX => config.numberOfGridCubeX;
    public int NumberOfGridCubeZ => config.numberOfGridCubeZ;

    private float minTemp;
    private float maxTemp;

    public void InitializeThermoGridSpawner(ConfigLoader loader)
    {
        configLoader = loader;
        if (configLoader == null)
        {
            Debug.LogError("ConfigLoader is not set. Ensure ConfigLoader is initialized.");
            return;
        }

        LoadConfig();
    }

    public void ReloadConfig()
    {
        LoadConfig();
    }

    private void LoadConfig()
    {
        if (configLoader == null)
        {
            Debug.LogError("ConfigLoader is not set. Ensure ConfigLoader is initialized.");
            return;
        }

        config = configLoader.LoadConfig<ThermoGridConfig>(configFileName);

        if (config == null)
        {
            Debug.LogError("Failed to load ThermoGridConfig.");
        }
    }

    public void InitializeGrid(Transform courtTransform)
    {
        if (courtTransform == null)
        {
            Debug.LogError("Court transform is null. Cannot initialize thermal grid.");
            return;
        }

        // Align the thermal grid parent with the court
        if (thermalGridParent == null)
        {
            thermalGridParent = new GameObject("ThermalGridParent");
            thermalGridParent.transform.SetParent(courtTransform);
            thermalGridParent.transform.localPosition = Vector3.zero;
        }

        // Get the floor's size and position
        Transform floorTransform = courtTransform.Find("Court_Floor");
        if (floorTransform != null)
        {
            floorSize = floorTransform.localScale;
            floorPosition = floorTransform.position;
        }
        else
        {
            Debug.LogError("Court_Floor not found in the CourtSpawner.");
            return;
        }

        GenerateGrid();
    }

    public void GenerateGrid()
    {
        // Initialize the temperature arrays
        areaTemp = new float[config.numberOfGridCubeX, config.numberOfGridCubeZ];
        baseTemp = new float[config.numberOfGridCubeX, config.numberOfGridCubeZ];

        // Set default temperatures
        for (int x = 0; x < config.numberOfGridCubeX; x++)
        {
            for (int z = 0; z < config.numberOfGridCubeZ; z++)
            {
                areaTemp[x, z] = config.fieldDefaultTemp;
                baseTemp[x, z] = config.fieldDefaultTemp;
                GameObject gridCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                gridCube.transform.position = CalculateGridCubePosition(x, z);
                gridCube.transform.localScale = CalculateGridCubeSize();
                gridCube.transform.SetParent(thermalGridParent.transform);
                gridCube.name = $"{x},0,{z}";
                gridCube.tag = "thermalGridCube";
                gridCube.layer = LayerMask.NameToLayer("Player");

                // Ensure the collider is set to trigger
                Collider collider = gridCube.GetComponent<Collider>();
                collider.isTrigger = true;

                // Optionally disable the renderer if you don't want to see the cubes
                gridCube.GetComponent<Renderer>().enabled = false;
            }
        }

        if (config.useRandomHotSpot)
        {
            SetHotSpotWithSize();
        }
        if (config.useObjectHotSpot)
        {
            ApplyObstacleTemperatures();
        }
        
        ApplyGaussianSmoothing();

        // After all modifications, copy areaTemp to baseTemp
        for (int x = 0; x < config.numberOfGridCubeX; x++)
        {
            for (int z = 0; z < config.numberOfGridCubeZ; z++)
            {
                baseTemp[x, z] = areaTemp[x, z];
            }
        }

        isThermalGridReady = true; // Mark as ready
    }

    // Function to reset the temperature of the grid cubes
    public void ResetGrid()
    {
        minTemp = FindAnyObjectByType<InteroceptiveAgent>().thermoLevelRange.min;
        maxTemp = FindAnyObjectByType<InteroceptiveAgent>().thermoLevelRange.max;

        // Reset default temperatures
        for (int x = 0; x < config.numberOfGridCubeX; x++)
        {
            for (int z = 0; z < config.numberOfGridCubeZ; z++)
            {
                areaTemp[x, z] = config.fieldDefaultTemp;
                baseTemp[x, z] = config.fieldDefaultTemp;
            }
        }

        Debug.Log("ThermoGridSpawner: Default temperatures reset.");

        if (config.useRandomHotSpot)
        {
            SetHotSpotWithSize();
        }
        if (config.useObjectHotSpot)
        {
            ApplyObstacleTemperatures();
        }

        ApplyGaussianSmoothing();

        // After all modifications, copy areaTemp to baseTemp
        for (int x = 0; x < config.numberOfGridCubeX; x++)
        {
            for (int z = 0; z < config.numberOfGridCubeZ; z++)
            {
                baseTemp[x, z] = areaTemp[x, z];
            }
        }

        Debug.Log("ThermoGridSpawner: Thermal grid has been reset.");
    }

    // Function to set the size of hot spots
    public void SetHotSpotWithSize()
    {
        for (int i = 0; i < config.hotSpotCount; i++)
        {
            int centerX = Random.Range(0, config.numberOfGridCubeX);
            int centerZ = Random.Range(0, config.numberOfGridCubeZ);

            int halfSize = (int)(config.hotSpotSize / 2);

            for (int x = centerX - halfSize; x <= centerX + halfSize; x++)
            {
                for (int z = centerZ - halfSize; z <= centerZ + halfSize; z++)
                {
                    if (x >= 0 && x < config.numberOfGridCubeX && z >= 0 && z < config.numberOfGridCubeZ)
                    {
                        areaTemp[x, z] = config.hotSpotTemp;
                    }
                }
            }
        }
    }

    // Function to apply Gaussian smoothing
    public void ApplyGaussianSmoothing()
    {
        if (config.smoothingSigma == 0)
        {
            Debug.Log("Gaussian smoothing is disabled because smoothingSigma is set to 0.");
            return;
        }

        float[,] smoothedTemp = new float[config.numberOfGridCubeX, config.numberOfGridCubeZ];
        for (int x = 0; x < config.numberOfGridCubeX; x++)
        {
            for (int z = 0; z < config.numberOfGridCubeZ; z++)
            {
                smoothedTemp[x, z] = CalculateGaussianValue(x, z);
            }
        }
        areaTemp = smoothedTemp;
    }

    // Function to calculate the Gaussian smoothed value for a grid cell
    // Gaussian smoothing is a technique used to smooth data by averaging nearby values with weights
    // determined by a Gaussian (normal) distribution. The Gaussian function is defined as:
    // 
    //     G(x, y) = exp(-(x^2 + y^2) / (2 * sigma^2))
    //
    // where (x, y) are the coordinates relative to the center of the kernel, and sigma is the standard
    // deviation of the Gaussian distribution. The weights decrease with distance from the center,
    // giving more importance to nearby values.
    //
    // In this function, we apply a Gaussian kernel to the grid cell at (x, z) to calculate the smoothed
    // temperature value.
    private float CalculateGaussianValue(int x, int z)
    {
        float sigma = config.smoothingSigma;
        int kernelRadius = Mathf.CeilToInt(3 * sigma); // Typically, 3*sigma is used as the radius
        float sum = 0f;
        float weightSum = 0f;

        // Iterate over the kernel area centered around the grid cell (x, z)
        for (int i = -kernelRadius; i <= kernelRadius; i++)
        {
            for (int j = -kernelRadius; j <= kernelRadius; j++)
            {
                int nx = x + i;
                int nz = z + j;
                if (nx >= 0 && nx < config.numberOfGridCubeX && nz >= 0 && nz < config.numberOfGridCubeZ)
                {
                    // Calculate the Euclidean distance from the center of the kernel
                    float distance = Mathf.Sqrt(i * i + j * j);
                    // Calculate the Gaussian weight based on the distance
                    float weight = Mathf.Exp(-(distance * distance) / (2 * sigma * sigma));
                    // Accumulate the weighted temperature values
                    sum += areaTemp[nx, nz] * weight;
                    // Accumulate the weights
                    weightSum += weight;
                }
            }
        }

        // Normalize the accumulated temperature values by the sum of the weights
        return sum / weightSum;
    }

    private Vector3 CalculateGridCubePosition(int x, int z)
    {
        Vector3 gridCubeSize = CalculateGridCubeSize();

        return new Vector3(
            floorPosition.x - floorSize.x / 2 + gridCubeSize.x / 2 + x * gridCubeSize.x,
            floorPosition.y + gridCubeSize.y / 2,
            floorPosition.z - floorSize.z / 2 + gridCubeSize.z / 2 + z * gridCubeSize.z
        );
    }

    private Vector3 CalculateGridCubeSize()
    {
        return new Vector3(
            floorSize.x / config.numberOfGridCubeX,
            config.gridCubeHeight,
            floorSize.z / config.numberOfGridCubeZ
        );
    }

    public float GetAreaTemp(int x, int z)
    {
        if (x >= 0 && x < config.numberOfGridCubeX && z >= 0 && z < config.numberOfGridCubeZ)
        {
            return areaTemp[x, z];
        }
        return config.fieldDefaultTemp;
    }

    public float GetNormalizedAreaTemp(int x, int z)
    {
        float minTemp = FindAnyObjectByType<InteroceptiveAgent>().thermoLevelRange.min; 
        float maxTemp = FindAnyObjectByType<InteroceptiveAgent>().thermoLevelRange.max;

        if (x >= 0 && x < config.numberOfGridCubeX && z >= 0 && z < config.numberOfGridCubeZ)
        {
            // float minTemp = config.fieldDefaultTemp;
            // float maxTemp = config.hotSpotTemp;
            return Mathf.InverseLerp(minTemp, maxTemp, areaTemp[x, z]);
        }
        return 0f;
    }


    public void ClearGrid()
    {
        if (thermalGridParent != null)
        {
            foreach (Transform child in thermalGridParent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        isThermalGridReady = false; // Mark as ready
    }

    // Function to adjust the temperature of the grid cubes
    public void AdjustTemperature(float temperatureChange)
    {
        for (int x = 0; x < config.numberOfGridCubeX; x++)
        {
            for (int z = 0; z < config.numberOfGridCubeZ; z++)
            {
                areaTemp[x, z] += temperatureChange;
            }
        }
        Debug.Log($"ThermoGridSpawner: Temperature adjusted by {temperatureChange}.");
    }

    private void ApplyObstacleTemperatures()
    {
        // var obstacles = FindObjectsOfType<ThermalObject>();
        // Debug.Log($"Found {obstacles.Length} ThermalObjects in the scene.");
        // foreach (var obstacle in obstacles)
        var obstacleSpawners = FindObjectsOfType<ObstacleSpawner>();
        List<GameObject> obstacles = new List<GameObject>();
        foreach (var spawner in obstacleSpawners)
        {
            obstacles.AddRange(spawner.GetSpawnedObstacles());
        }
        foreach (var obstacle in obstacles)
        {
            if (obstacle == null || !obstacle.gameObject.activeInHierarchy)
            {
                continue;
            }

            ObstacleTemperature obstacleTemp = obstacle.GetComponent<ObstacleTemperature>();
            if (obstacleTemp == null || obstacleTemp.temperature == 0)
            {
                continue;
            }

            // if (obstacle.temperature == 0)
            // {
            //     continue;
            // }

            Collider collider = obstacle.GetComponent<Collider>();
            if (collider == null)
            {
                Debug.LogWarning($"Collider not found on obstacle: {obstacle.name}");
                continue;
            }

            Vector3 obstaclePosition = obstacle.transform.position;
            Vector3 obstacleSize = collider.bounds.size;

            int minX = Mathf.FloorToInt((obstaclePosition.x - obstacleSize.x / 2 - floorPosition.x + floorSize.x / 2) / CalculateGridCubeSize().x);
            int maxX = Mathf.FloorToInt((obstaclePosition.x + obstacleSize.x / 2 - floorPosition.x + floorSize.x / 2) / CalculateGridCubeSize().x);
            int minZ = Mathf.FloorToInt((obstaclePosition.z - obstacleSize.z / 2 - floorPosition.z + floorSize.z / 2) / CalculateGridCubeSize().z);
            int maxZ = Mathf.FloorToInt((obstaclePosition.z + obstacleSize.z / 2 - floorPosition.z + floorSize.z / 2) / CalculateGridCubeSize().z);

            for (int x = minX; x <= maxX; x++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    if (x >= 0 && x < config.numberOfGridCubeX && z >= 0 && z < config.numberOfGridCubeZ)
                    {
                        // areaTemp[x, z] = obstacle.temperature;
                        areaTemp[x, z] = obstacleTemp.temperature;
                    }
                }
            }
        }
    }

    // New method to set temperature for day/night
    public void SetDayNightTemperature(float offset)
    {
        for (int x = 0; x < config.numberOfGridCubeX; x++)
        {
            for (int z = 0; z < config.numberOfGridCubeZ; z++)
            {
                areaTemp[x, z] = baseTemp[x, z] + offset;
            }
        }
        // Debug.Log($"ThermoGridSpawner: Set temperature offset {offset} for day/night.");
    }
}
