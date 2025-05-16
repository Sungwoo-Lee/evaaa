using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Utility;

[System.Serializable]
public class LandmarkConfig
{
    public List<int> customPattern;
    public int patternRows;
    public int patternCols;
    public float landmarkRadius;
    public float overlapPadding;
}

public class LandmarkSpawner : MonoBehaviour
{
    [Header("Landmark Config Settings")]
    public string configFileName = "landmarkConfig.json";
    public string prefabFolder = "Obstacles";
    public GameObject defaultLandmarkPrefab;

    private LandmarkConfig landmarkConfig;
    private ConfigLoader configLoader;
    private List<GameObject> spawnedLandmarks = new List<GameObject>();
    // List of points forming the convex hull (polygonal area) around all landmarks
    private List<Vector3> convexHullPoints = new List<Vector3>();
    private GameObject landmarkAreaMeshObj; // Holds the MeshCollider for the convex hull area
    private CourtSpawner courtSpawner; // Reference to CourtSpawner for court size
    private int[,] customPattern;
    private float landmarkRadius;
    private float overlapPadding;
    private int patternRows;
    private int patternCols;

    public Vector3 regionMin = new Vector3(-15, 0, -15);
    public Vector3 regionMax = new Vector3(15, 0, 15);

    public LayerMask staticObjectLayerMask; // Assign in inspector to include static objects/resources

    public void InitializeLandmarkSpawner(ConfigLoader loader, Transform parent)
    {
        configLoader = loader;
        if (configLoader == null)
        {
            Debug.LogError("ConfigLoader is not set. Ensure ConfigLoader is initialized.");
            return;
        }
        LoadConfig();
        GenerateLandmarksGrid();
        // After spawning, compute the convex hull (polygonal area)
        ComputeConvexHull();
        // After computing the hull, generate the MeshCollider
        CreateConvexHullMeshCollider();
    }

    private void LoadConfig()
    {
        string configPath = configLoader.GetFullPath(configFileName);
        Debug.Log($"LandmarkSpawner: Attempting to load config from: {configPath}");
        if (System.IO.File.Exists(configPath))
        {
            string rawJson = System.IO.File.ReadAllText(configPath);
            Debug.Log($"LandmarkSpawner: Raw JSON content: {rawJson}");
        }
        else
        {
            Debug.LogError($"LandmarkSpawner: Config file does not exist at: {configPath}");
        }
        landmarkConfig = configLoader.LoadConfig<LandmarkConfig>(configFileName);
        if (landmarkConfig == null || landmarkConfig.customPattern == null)
        {
            Debug.LogError("Invalid or empty landmark configuration or customPattern.");
        }
        else
        {
            patternRows = landmarkConfig.patternRows;
            patternCols = landmarkConfig.patternCols;
            customPattern = new int[patternRows, patternCols];
            for (int i = 0; i < patternRows; i++)
                for (int j = 0; j < patternCols; j++)
                {
                    int flatIndex = i * patternCols + j; // config index
                    int unityRow = patternRows - 1 - i;  // reverse row for Unity Z
                    customPattern[unityRow, j] = landmarkConfig.customPattern[flatIndex];
                }
            landmarkRadius = landmarkConfig.landmarkRadius;
            overlapPadding = landmarkConfig.overlapPadding;
        }
    }

    public void GenerateLandmarksGrid()
    {
        if (courtSpawner == null || courtSpawner.courtConfig == null) {
            Debug.LogError("CourtSpawner or its config is not set. Cannot generate grid landmarks.");
            return;
        }
        GameObject overlapCheckPrefab = defaultLandmarkPrefab;
        if (overlapCheckPrefab == null)
        {
            overlapCheckPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            overlapCheckPrefab.transform.localScale = Vector3.one * landmarkRadius * 2f;
            overlapCheckPrefab.GetComponent<Collider>().isTrigger = true;
            overlapCheckPrefab.SetActive(false);
        }
        Vector3 floorSize = courtSpawner.courtConfig.floorSize;
        Vector3 courtOrigin = courtSpawner.courtConfig.position;
        int rows = customPattern.GetLength(0);
        int cols = customPattern.GetLength(1);
        float cellWidth = floorSize.x / cols;
        float cellHeight = floorSize.z / rows;
        float y = courtOrigin.y;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int n = customPattern[row, col];
                if (n <= 0) continue;
                float cellXMin = courtOrigin.x - floorSize.x / 2 + col * cellWidth;
                float cellZMin = courtOrigin.z - floorSize.z / 2 + row * cellHeight;
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        float x = cellXMin + cellWidth * (i + 0.5f) / n;
                        float z = cellZMin + cellHeight * (j + 0.5f) / n;
                        Vector3 pos = new Vector3(x, y, z);
                        Collider[] staticOverlaps = Physics.OverlapSphere(pos, landmarkRadius + overlapPadding, staticObjectLayerMask);
                        if (staticOverlaps.Length > 0)
                        {
                            continue;
                        }
                        if (!OverlapUtility.IsOverlapping(
                                pos,
                                overlapCheckPrefab,
                                overlapCheckPrefab.transform.localScale,
                                1.0f,
                                overlapPadding))
                        {
                            GameObject landmark = defaultLandmarkPrefab ?
                                Instantiate(defaultLandmarkPrefab, pos, Quaternion.identity, transform) :
                                new GameObject($"Landmark_{row}_{col}_{i}_{j}");
                            landmark.transform.position = pos;
                            landmark.transform.parent = transform;
                            var colComp = landmark.AddComponent<SphereCollider>();
                            colComp.radius = landmarkRadius;
                            colComp.isTrigger = true;
                            spawnedLandmarks.Add(landmark);
                        }
                    }
                }
            }
        }
        if (defaultLandmarkPrefab == null && overlapCheckPrefab != null)
        {
            DestroyImmediate(overlapCheckPrefab);
        }
    }

    public Bounds GetLandmarkAreaBounds()
    {
        return new Bounds();
    }

    public void RemoveLandmarkColliders()
    {
        foreach (var landmark in spawnedLandmarks)
        {
            var col = landmark.GetComponent<Collider>();
            if (col) Destroy(col);
        }
    }

    public List<GameObject> GetLandmarks() => spawnedLandmarks;

    /// <summary>
    /// Returns the convex hull points for logic or visualization
    /// </summary>
    public List<Vector3> GetConvexHullPoints() => convexHullPoints;

    /// <summary>
    /// Computes the convex hull (smallest convex polygon) that contains all landmark positions.
    /// Uses the Graham scan (Andrew's monotone chain) algorithm in the XZ plane.
    /// The convex hull is used to tightly wrap the landmark area, avoiding obstacles.
    /// </summary>
    private void ComputeConvexHull()
    {
        convexHullPoints.Clear();
        if (spawnedLandmarks.Count < 3) return; // Need at least 3 points for a polygon
        // 1. Collect all landmark positions as 2D points (XZ plane)
        List<Vector2> points2D = new List<Vector2>();
        Dictionary<Vector2, Vector3> pointMap = new Dictionary<Vector2, Vector3>();
        foreach (var lm in spawnedLandmarks)
        {
            Vector2 p2d = new Vector2(lm.transform.position.x, lm.transform.position.z);
            points2D.Add(p2d);
            pointMap[p2d] = lm.transform.position;
        }
        // 2. Sort points by X, then by Y (required for the algorithm)
        points2D.Sort((a, b) => a.x == b.x ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x));
        List<Vector2> hull = new List<Vector2>();
        // 3. Build the lower hull
        foreach (var p in points2D)
        {
            // Remove last point if it would cause a right turn (not convex)
            while (hull.Count >= 2 && Cross(hull[hull.Count-2], hull[hull.Count-1], p) <= 0)
                hull.RemoveAt(hull.Count-1);
            hull.Add(p);
        }
        // 4. Build the upper hull
        int t = hull.Count + 1;
        for (int i = points2D.Count - 2; i >= 0; i--)
        {
            var p = points2D[i];
            while (hull.Count >= t && Cross(hull[hull.Count-2], hull[hull.Count-1], p) <= 0)
                hull.RemoveAt(hull.Count-1);
            hull.Add(p);
        }
        hull.RemoveAt(hull.Count-1); // Remove duplicate last point
        // 5. Convert hull points back to Vector3 (using the Y of the first landmark)
        float yLevel = spawnedLandmarks.Count > 0 ? spawnedLandmarks[0].transform.position.y : 0f;
        foreach (var p2d in hull)
        {
            convexHullPoints.Add(new Vector3(p2d.x, yLevel, p2d.y));
        }
        // Debug.Log($"[LandmarkSpawner] Number of convex hull points: {convexHullPoints.Count}");
    }

    /// <summary>
    /// Helper function for convex hull: returns the cross product of OA and OB vectors.
    /// Positive if OAB makes a left turn, negative for right turn, zero if collinear.
    /// </summary>
    private float Cross(Vector2 o, Vector2 a, Vector2 b)
    {
        return (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x);
    }

    /// <summary>
    /// Generates a MeshCollider (convex, trigger) from the convex hull points.
    /// The mesh is a flat polygon in the XZ plane, triangulated using fan triangulation.
    /// </summary>
    private void CreateConvexHullMeshCollider()
    {
        // Remove old mesh object if it exists
        if (landmarkAreaMeshObj != null)
        {
            DestroyImmediate(landmarkAreaMeshObj);
        }
        if (convexHullPoints == null || convexHullPoints.Count < 3) return;

        // Create new GameObject for the mesh
        landmarkAreaMeshObj = new GameObject("LandmarkAreaMesh");
        landmarkAreaMeshObj.layer = LayerMask.NameToLayer("LandmarkArea");
        landmarkAreaMeshObj.transform.parent = this.transform;
        landmarkAreaMeshObj.transform.position = Vector3.zero;

        // Create mesh
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[convexHullPoints.Count];
        for (int i = 0; i < convexHullPoints.Count; i++)
        {
            // Flatten to a fixed Y (use the Y of the first landmark, or 0)
            float yLevel = spawnedLandmarks.Count > 0 ? spawnedLandmarks[0].transform.position.y : 0f;
            vertices[i] = new Vector3(convexHullPoints[i].x, yLevel, convexHullPoints[i].z);
        }
        // Fan triangulation: (0, i, i+1)
        int[] triangles = new int[(convexHullPoints.Count - 2) * 3];
        for (int i = 1; i < convexHullPoints.Count - 1; i++)
        {
            triangles[(i - 1) * 3 + 0] = 0;
            triangles[(i - 1) * 3 + 1] = i;
            triangles[(i - 1) * 3 + 2] = i + 1;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Add MeshCollider
        MeshCollider meshCollider = landmarkAreaMeshObj.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
        meshCollider.isTrigger = true;

        // DEBUG: Add MeshRenderer and MeshFilter to visualize the mesh in the Scene view
        var meshFilter = landmarkAreaMeshObj.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        var meshRenderer = landmarkAreaMeshObj.AddComponent<MeshRenderer>();
        // Use a semi-transparent green material
        var debugMat = new Material(Shader.Find("Standard"));
        debugMat.color = new Color(0, 1, 0, 0.3f); // RGBA, alpha=0.3 for transparency
        debugMat.SetFloat("_Mode", 3); // Transparent mode
        debugMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        debugMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        debugMat.SetInt("_ZWrite", 0);
        debugMat.DisableKeyword("_ALPHATEST_ON");
        debugMat.EnableKeyword("_ALPHABLEND_ON");
        debugMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        debugMat.renderQueue = 3000;
        meshRenderer.material = debugMat;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Draws the convex hull polygon in the Scene view using green lines for visualization.
    /// The polygon is drawn slightly above ground for visibility.
    /// </summary>
    private void OnDrawGizmos()
    {
        // Always recompute in editor for visualization
        if (spawnedLandmarks != null && spawnedLandmarks.Count >= 3)
            ComputeConvexHull();

        if (convexHullPoints != null && convexHullPoints.Count > 1)
        {
            Gizmos.color = Color.green;
            float yOffset = 1.0f; // Raise above ground for visibility
            for (int i = 0; i < convexHullPoints.Count; i++)
            {
                Vector3 from = convexHullPoints[i] + Vector3.up * yOffset;
                Vector3 to = convexHullPoints[(i+1)%convexHullPoints.Count] + Vector3.up * yOffset;
                Gizmos.DrawLine(from, to);
            }
        }
    }
#endif

    public void SetCourtSpawner(CourtSpawner court) {
        courtSpawner = court;
    }

    // Grid-based landmark generation (original version)
    
} 