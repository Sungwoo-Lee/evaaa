using UnityEngine;
using System.IO;

[System.Serializable]
public class CourtConfig
{
    public Vector3 floorSize;    // Size of the floor (X, Y, Z)
    public float wallHeight;     // Height of the walls
    public Vector3 position;     // Position of the Court
    public string floorMaterialName; // Name of the floor material in Resources/Materials
    public string wallMaterialName;  // Name of the wall material in Resources/Materials
    public bool createWall = true;   // Whether to create walls around the court
}

public class CourtSpawner : MonoBehaviour
{
    public string configFileName = "courtConfig.json";
    private Material floorMaterial;
    private Material wallMaterial;

    public CourtConfig courtConfig; // Court configuration data
    private GameObject courtObject;  // Store the generated court
    private GameObject courtFloor;   // Store the generated court floor

    private ConfigLoader configLoader; // Reference to ConfigLoader

    public Transform CourtTransform => courtObject != null ? courtObject.transform : null;
    public Transform CourtFloorTransform => courtFloor != null ? courtFloor.transform : null;

    public void InitializeCourt(ConfigLoader loader)
    {
        configLoader = loader;
        if (configLoader == null)
        {
            Debug.LogError("ConfigLoader is not set. Ensure ConfigLoader is initialized.");
            return;
        }

        LoadConfig();

        if (courtConfig == null)
        {
            Debug.LogError("Court configuration is not loaded. Call ReloadConfig() before InitializeCourt().");
            return;
        }

        LoadMaterials();
        GenerateCourt();
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

        courtConfig = configLoader.LoadConfig<CourtConfig>(configFileName);

        if (courtConfig == null)
        {
            Debug.LogError("Invalid court configuration.");
        }
    }

    private void LoadMaterials()
    {
        if (!string.IsNullOrEmpty(courtConfig.floorMaterialName))
        {
            floorMaterial = Resources.Load<Material>($"Materials/{courtConfig.floorMaterialName}");
            if (floorMaterial == null)
            {
                Debug.LogError($"Floor material not found: {courtConfig.floorMaterialName}");
            }
        }
        else
        {
            Debug.LogError("Floor material name is not specified in configuration.");
        }

        if (!string.IsNullOrEmpty(courtConfig.wallMaterialName))
        {
            wallMaterial = Resources.Load<Material>($"Materials/{courtConfig.wallMaterialName}");
            if (wallMaterial == null)
            {
                Debug.LogError($"Wall material not found: {courtConfig.wallMaterialName}");
            }
        }
        else
        {
            Debug.LogError("Wall material name is not specified in configuration.");
        }
    }

    private void GenerateCourt()
    {
        // Clear existing court
        if (courtObject != null)
        {
            Destroy(courtObject);
        }

        courtObject = new GameObject("Court");
        courtObject.transform.position = courtConfig.position;

        CreateFloor(courtObject.transform);
        CreateWalls(courtObject.transform);

        // Debug.Log("Court generated.");
    }

    private void CreateFloor(Transform parent)
    {
        courtFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        courtFloor.name = "Court_Floor";
        courtFloor.transform.parent = parent;
        courtFloor.transform.localScale = new Vector3(courtConfig.floorSize.x, 1, courtConfig.floorSize.z);

        if (floorMaterial != null)
        {
            courtFloor.GetComponent<Renderer>().material = floorMaterial;
        }
    }

    private void CreateWalls(Transform parent)
    {
        if (!courtConfig.createWall)
        {
            return;
        }

        float halfX = courtConfig.floorSize.x / 2;
        float halfZ = courtConfig.floorSize.z / 2;
        float wallThickness = 1;

        Vector3[] wallPositions =
        {
            new Vector3(0, courtConfig.wallHeight / 2, halfZ + wallThickness / 2),
            new Vector3(0, courtConfig.wallHeight / 2, -(halfZ + wallThickness / 2)),
            new Vector3(halfX + wallThickness / 2, courtConfig.wallHeight / 2, 0),
            new Vector3(-(halfX + wallThickness / 2), courtConfig.wallHeight / 2, 0)
        };

        Vector3[] wallSizes =
        {
            new Vector3(courtConfig.floorSize.x, courtConfig.wallHeight, wallThickness),
            new Vector3(courtConfig.floorSize.x, courtConfig.wallHeight, wallThickness),
            new Vector3(wallThickness, courtConfig.wallHeight, courtConfig.floorSize.z),
            new Vector3(wallThickness, courtConfig.wallHeight, courtConfig.floorSize.z)
        };

        string[] wallNames = { "Wall_North", "Wall_South", "Wall_East", "Wall_West" };

        for (int i = 0; i < 4; i++)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = $"Court_{wallNames[i]}";
            wall.transform.parent = parent;
            wall.transform.localPosition = wallPositions[i];
            wall.transform.localScale = wallSizes[i];

            if (wallMaterial != null)
            {
                wall.GetComponent<Renderer>().material = wallMaterial;
            }
        }
    }
}
