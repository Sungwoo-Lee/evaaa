using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utility;

public enum ResourceType
{
    Static,        // Static resource (e.g., Pond)
    Random,        // Random resource (e.g., Water)
    GroupedRandom  // Grouped random resource (e.g., Apple)
}

[System.Serializable]
public class ResourceGroup
{
    public string prefabName;
    public string prefabLabel;
    public int count;
    public PositionRange position;
    public RotationRange rotationRange;
    public ScaleRange scaleRange;
    public string resourceType; // Directly use resourceType from JSON
    public ResourceType parsedResourceType; // Parsed enum value
}

[System.Serializable]
public class ResourceConfig
{
    public List<ResourceGroup> groups;
}

public class ResourceSpawner : MonoBehaviour
{
    [Header("Configuration")]
    public string configFileName = "resourceConfig.json";
    public string prefabFolder = "Resources";

    private ResourceConfig resourceConfig = new ResourceConfig(); // Parsed resource configuration
    private List<GameObject> spawnedResources = new List<GameObject>(); // Tracks generated resources
    private Transform courtTransform = null; // Reference to the court object for parenting resources

    private ConfigLoader configLoader; // Reference to ConfigLoader
    private ResourceGroup currentLocationGroup; // Currently selected GroupedRandom location
    private List<ResourceGroup> staticGroups; // Static resources
    private List<ResourceGroup> randomGroups; // Random resources
    private List<ResourceGroup> groupedRandomGroups; // GroupedRandom resources

    private int activeResourcesInCurrentGroup; // Tracks active resources in the current GroupedRandom group
    private bool isLocationLocked = false; // Indicates if the current GroupedRandom location is locked

    private bool onlyStaticMode = false;

    public void InitializeResourceSpawner(ConfigLoader loader, bool onlyStatic = false)
    {
        configLoader = loader;
        if (configLoader == null)
        {
            Debug.LogError("ConfigLoader is not set. Ensure ConfigLoader is initialized.");
            return;
        }

        LoadConfig();
        InitializeAvailableGroups();
        SelectRandomLocation();
        this.onlyStaticMode = onlyStatic;
        Debug.Log($"ResourceSpawner: Initializing phase: {(onlyStatic ? "STATIC" : "RANDOM/GROUPED")}");
    }

    public void ReloadConfig()
    {
        LoadConfig();
        InitializeAvailableGroups();
        SelectRandomLocation();
    }

    public void InitializeResources(Transform court, bool onlyStatic = false)
    {
        if (resourceConfig == null)
        {
            Debug.LogError("Resource configuration is not loaded. Call ReloadConfig() before InitializeResources().");
            return;
        }

        courtTransform = court;

        if (onlyStatic)
        {
            Debug.Log($"ResourceSpawner: Spawning STATIC resources: {staticGroups.Count} groups");
            // Generate resources for static groups (if any)
            if (staticGroups.Count > 0)
            {
                foreach (var group in staticGroups)
                {
                    Debug.Log($"ResourceSpawner: Spawning static resource group: {group.prefabLabel} (count: {group.count})");
                    SpawnResourceGroup(group);
                }
            }
            else
            {
                Debug.Log("No Static groups available. Skipping Static resource initialization.");
            }
            return;
        }

        Debug.Log($"ResourceSpawner: Spawning RANDOM resources: {randomGroups.Count} groups");
        // Generate resources for random groups (if any)
        if (randomGroups.Count > 0)
        {
            foreach (var group in randomGroups)
            {
                Debug.Log($"ResourceSpawner: Spawning random resource group: {group.prefabLabel} (count: {group.count})");
                SpawnResourceGroup(group);
            }
        }
        else
        {
            Debug.Log("No Random groups available. Skipping Random resource initialization.");
        }

        Debug.Log($"ResourceSpawner: Spawning GROUPED RANDOM resources: {groupedRandomGroups.Count} groups");
        // Generate resources for the first GroupedRandom group (if any)
        if (groupedRandomGroups.Count > 0)
        {
            SelectRandomLocation();
            Debug.Log($"ResourceSpawner: Spawning grouped random resource group: {currentLocationGroup.prefabLabel} (count: {currentLocationGroup.count})");
            GenerateGroupedRandomResources(); // Only generate resources for the selected GroupedRandom group
            activeResourcesInCurrentGroup = currentLocationGroup.count; // Initialize the counter
            isLocationLocked = true; // Lock the location
        }
        else
        {
            Debug.Log("No GroupedRandom groups available. Skipping GroupedRandom resource initialization.");
        }
    }

    private void LoadConfig()
    {
        if (configLoader == null)
        {
            Debug.LogError("ConfigLoader is not set. Ensure ConfigLoader is initialized.");
            return;
        }

        resourceConfig = configLoader.LoadConfig<ResourceConfig>(configFileName);

        if (resourceConfig == null || resourceConfig.groups == null)
        {
            Debug.LogError("Invalid or empty resource configuration.");
            return;
        }

        // Parse the resourceType field and assign it to parsedResourceType
        foreach (var group in resourceConfig.groups)
        {
            try
            {
                group.parsedResourceType = (ResourceType)System.Enum.Parse(typeof(ResourceType), group.resourceType, true);
                Debug.Log($"Loaded resource group: {group.prefabLabel}, Type: {group.parsedResourceType}, Count: {group.count}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to parse resourceType for group {group.prefabLabel}: {ex.Message}");
            }
        }
    }

    private void InitializeAvailableGroups()
    {
        staticGroups = resourceConfig.groups?.Where(g => g.parsedResourceType == ResourceType.Static).ToList() ?? new List<ResourceGroup>();
        randomGroups = resourceConfig.groups?.Where(g => g.parsedResourceType == ResourceType.Random).ToList() ?? new List<ResourceGroup>();
        groupedRandomGroups = resourceConfig.groups?.Where(g => g.parsedResourceType == ResourceType.GroupedRandom).ToList() ?? new List<ResourceGroup>();

        Debug.Log($"Static groups: {staticGroups.Count}, Random groups: {randomGroups.Count}, GroupedRandom groups: {groupedRandomGroups.Count}");
    }

    private void SelectRandomLocation()
    {
        if (groupedRandomGroups == null || groupedRandomGroups.Count == 0)
        {
            Debug.LogWarning("No GroupedRandom groups available. Skipping random location selection.");
            return; // Skip if no GroupedRandom groups are available
        }

        // Randomly select a location group from groupedRandomGroups
        currentLocationGroup = groupedRandomGroups[Random.Range(0, groupedRandomGroups.Count)];
        Debug.Log($"Selected GroupedRandom location: {currentLocationGroup.prefabLabel}");
    }

    public void ClearAllResources()
    {
        ClearResources();
        Resources.UnloadUnusedAssets();
        Debug.Log("ResourceSpawner: All resources have been cleared.");
    }

    public void ClearRandomResources()
    {
        ClearResources();
        Resources.UnloadUnusedAssets();
        Debug.Log("ResourceSpawner: Random resources have been cleared.");
    }

    public void ResetResources()
    {
        // Reset all resources (Static, Random, and GroupedRandom)
        ClearResources();

        // Regenerate all resources
        foreach (var group in staticGroups) SpawnResourceGroup(group);
        foreach (var group in randomGroups) SpawnResourceGroup(group);
        if (groupedRandomGroups.Count > 0)
        {
            SelectRandomLocation();
            GenerateGroupedRandomResources();
        }

        Debug.Log("Scene reset completed.");
    }

    public void RelocateResource(Collider resource)
    {
        string resourceName = resource.name.Replace("(Clone)", "").Trim();

        ResourceGroup group = resourceConfig.groups?.Find(g => g.prefabLabel == resourceName);
        if (group == null)
        {
            Debug.LogError($"Resource {resourceName} configuration not found.");
            return;
        }

        switch (group.parsedResourceType)
        {
            case ResourceType.Static:
                // Debug.Log($"Static resource {resourceName} does not relocate.");
                break;

            case ResourceType.Random:
                Vector3 newPosition = RandomPosition(group.position);
                resource.transform.position = newPosition;
                // Debug.Log($"Random resource {resourceName} relocated to {newPosition}.");
                break;

            case ResourceType.GroupedRandom:
                resource.gameObject.SetActive(false); // Deactivate the consumed resource
                activeResourcesInCurrentGroup--;

                if (activeResourcesInCurrentGroup <= 0)
                {
                    Debug.Log($"All resources in {currentLocationGroup.prefabLabel} consumed.");
                    isLocationLocked = false;

                    if (!isLocationLocked && groupedRandomGroups.Count > 0)
                    {
                        ResetGroupedRandomResources();
                    }
                }
                break;
        }
    }

    private void ClearResources()
    {
        foreach (var resource in spawnedResources)
        {
            if (resource != null) Destroy(resource);
        }
        spawnedResources.Clear();
    }

    private void ResetGroupedRandomResources()
    {
        ClearGroupedRandomResources();
        SelectRandomLocation();
        GenerateGroupedRandomResources();
        activeResourcesInCurrentGroup = currentLocationGroup.count;
        isLocationLocked = true;
    }

    private void ClearGroupedRandomResources()
    {
        var groupedRandomResources = spawnedResources.Where(resource =>
        {
            if (resource == null) return false;

            string resourceName = resource.name.Replace("(Clone)", "").Trim();
            return groupedRandomGroups.Any(group => group.prefabLabel == resourceName);
        }).ToList();

        foreach (var resource in groupedRandomResources)
        {
            if (resource != null) Destroy(resource);
            spawnedResources.Remove(resource);
        }
    }

    private void GenerateGroupedRandomResources()
    {
        if (currentLocationGroup == null)
        {
            Debug.LogWarning("No GroupedRandom location group selected. Skipping resource generation.");
            return;
        }

        SpawnResourceGroup(currentLocationGroup);
    }

    private void SpawnResourceGroup(ResourceGroup group)
    {
        GameObject prefab = Resources.Load<GameObject>($"{prefabFolder}/{group.prefabName}");

        if (string.IsNullOrEmpty(group.prefabLabel))
        {
            group.prefabLabel = group.prefabName;
        }
        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: {group.prefabName}");
            return;
        }

        for (int i = 0; i < group.count; i++)
        {
            Vector3 position = RandomPosition(group.position);
            Quaternion rotation = RandomRotation(group.rotationRange);
            Vector3 scale = RandomScale(group.scaleRange);

            GameObject resource = Instantiate(prefab, position, rotation);
            resource.name = $"{group.prefabLabel}(Clone)";

            if (courtTransform != null)
            {
                resource.transform.SetParent(courtTransform);
            }

            resource.transform.localScale = scale;

            ResourceProperty resourceProperty = resource.GetComponentInChildren<ResourceProperty>();
            if (resourceProperty != null)
            {
                resourceProperty.InitializeProperties();
            }

            spawnedResources.Add(resource);
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
}
