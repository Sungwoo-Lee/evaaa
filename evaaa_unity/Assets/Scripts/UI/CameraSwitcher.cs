using UnityEngine;
using UnityEngine.UI;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("Reference to the third-person camera.")]
    public Camera thirdPersonCamera; // Reference to the third-person camera

    [Tooltip("Reference to the agent's first-person RawImage.")]
    public RawImage agentView; // The RawImage displaying the agent's first-person view

    [Tooltip("Key to switch views.")]
    public KeyCode switchKey = KeyCode.Y; // Key to switch views (default: Y)

    [Header("UI Panels")]
    [Tooltip("Reference to the panel containing the RadialMeters.")]
    public GameObject radialMeterPanel; // Reference to the panel containing the RadialMeters
    public GameObject thermoSensorGridPanel; // Reference to the panel containing the RadialMeters
    public GameObject totalRewardText; // Reference to the panel containing the RadialMeters

    [Header("Control Mode")]
    [Tooltip("Set to true if the agent is controlled by AI.")]
    public bool isAIControlled = false; // Determines if the agent is AI-controlled

    [Header("HeatMap Reference")]
    [Tooltip("Reference to the HeatMap script.")]
    public HeatMap heatMapScript; // Reference to the HeatMap script

    [Header("ResourceUI Reference")]
    [Tooltip("Reference to the ResourceUI script.")]
    public ResourceUI resourceUIScript; // Reference to the ResourceUI script

    // Internal state tracking
    private bool isFirstPersonFullScreen = false; // Tracks current view state
    private RectTransform agentViewRectTransform; // Cached RectTransform of agentView

    // Store original camera settings for restoration
    private LayerMask originalCullingMask;
    private CameraClearFlags originalClearFlags;

    void Awake()
    {
        // Ensure required references are assigned
        if (thirdPersonCamera == null)
        {
            Debug.LogError("Third Person Camera is not assigned in the Inspector.");
        }

        if (agentView == null)
        {
            Debug.LogError("Agent View (RawImage) is not assigned in the Inspector.");
        }

        if (radialMeterPanel == null)
        {
            Debug.LogError("Radial Meter Panel is not assigned in the Inspector.");
        }

        // Store the original camera settings
        originalCullingMask = thirdPersonCamera.cullingMask;
        originalClearFlags = thirdPersonCamera.clearFlags;
    }

    void Start()
    {
        // Cache the RectTransform component for efficiency
        agentViewRectTransform = agentView.GetComponent<RectTransform>();
        heatMapScript = FindObjectOfType<HeatMap>();
        resourceUIScript = FindObjectOfType<ResourceUI>();

        // Initialize the view based on control mode
        if (isAIControlled)
        {
            InitializeThirdPersonView();
        }
        else
        {
            InitializeFirstPersonView();
        }
    }

    void Update()
    {
        // Listen for the "Y" key press to toggle views (only for human players)
        if (Input.GetKeyDown(switchKey))
        {
            ToggleView();
        }
    }

    /// <summary>
    /// Initializes the camera and UI for the first-person view.
    /// </summary>
    public void InitializeFirstPersonView()
    {
        isFirstPersonFullScreen = true; // Set the initial state to first-person view

        // Expand the RawImage to cover the entire screen
        agentViewRectTransform.anchorMin = new Vector2(0, 0);
        agentViewRectTransform.anchorMax = new Vector2(1, 1);
        agentViewRectTransform.offsetMin = Vector2.zero;
        agentViewRectTransform.offsetMax = Vector2.zero;

        // Adjust the third-person camera to render only UI
        thirdPersonCamera.cullingMask = (1 << LayerMask.NameToLayer("UI")); // Render only UI layer
        thirdPersonCamera.clearFlags = CameraClearFlags.Depth; // Maintain existing depth settings

        // Activate the RadialMeterPanel
        if (radialMeterPanel != null)
        {
            radialMeterPanel.SetActive(true);
        }
        thermoSensorGridPanel.SetActive(true);

        if (totalRewardText != null)
        {
            totalRewardText.SetActive(true);
        }

        // Deactivate the heatmap and agent track in first-person view
        if (heatMapScript != null)
        {
            if (heatMapScript.heatMap != null)
                heatMapScript.heatMap.enabled = false;
            if (heatMapScript.agentTrack != null)
                heatMapScript.agentTrack.SetActive(false);
        }
        // Deactivate ResourceUI in first-person view
        if (resourceUIScript != null)
        {
            resourceUIScript.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Initializes the camera and UI for the third-person view.
    /// </summary>
    public void InitializeThirdPersonView()
    {
        isFirstPersonFullScreen = false; // Set the initial state to third-person view

        // Restore the RawImage to its original size and position
        agentViewRectTransform.anchorMin = new Vector2(0.8f, 0.1f); // Original anchorMin
        agentViewRectTransform.anchorMax = new Vector2(0.95f, 0.3f); // Original anchorMax
        agentViewRectTransform.offsetMin = Vector2.zero;
        agentViewRectTransform.offsetMax = Vector2.zero;

        // Restore the third-person camera's original culling mask and clear flags
        thirdPersonCamera.cullingMask = originalCullingMask;
        thirdPersonCamera.clearFlags = originalClearFlags;

        // Deactivate the RadialMeterPanel
        if (radialMeterPanel != null)
        {
            radialMeterPanel.SetActive(false);
        }
        thermoSensorGridPanel.SetActive(false);

        if (totalRewardText != null)
        {
            totalRewardText.SetActive(false);
        }

        // Reactivate the heatmap and agent track in third-person view
        if (heatMapScript != null)
        {
            if (heatMapScript.heatMap != null)
                heatMapScript.heatMap.enabled = true;
            if (heatMapScript.agentTrack != null)
                heatMapScript.agentTrack.SetActive(true);
        }
        // Reactivate ResourceUI in third-person view
        if (resourceUIScript != null)
        {
            resourceUIScript.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Toggles between first-person and third-person views when the switch key is pressed.
    /// </summary>
    void ToggleView()
    {
        // Toggle the view state
        isFirstPersonFullScreen = !isFirstPersonFullScreen;

        if (isFirstPersonFullScreen)
        {
            InitializeFirstPersonView();
        }
        else
        {
            InitializeThirdPersonView();
        }
    }
}


// using UnityEngine;
// using UnityEngine.UI;

// public class CameraSwitcher : MonoBehaviour
// {
//     [Header("Camera Settings")]
//     [Tooltip("Reference to the third-person camera.")]
//     public Camera thirdPersonCamera;       // Reference to the third-person camera

//     [Tooltip("Reference to the agent's first-person RawImage.")]
//     public RawImage agentView;             // The RawImage displaying the agent's first-person view

//     [Tooltip("Key to switch views.")]
//     public KeyCode switchKey = KeyCode.Y;  // Key to switch views (default: Y)

//     [Header("UI Panels")]
//     [Tooltip("Reference to the panel containing the RadialMeters.")]
//     public GameObject radialMeterPanel;    // Reference to the panel containing the RadialMeters
//     public GameObject thermoSensorGridPanel;    // Reference to the panel containing the RadialMeters

//     // Internal state tracking
//     private bool isFirstPersonFullScreen = false; // Tracks current view state

//     private RectTransform agentViewRectTransform; // Cached RectTransform of agentView

//     // Store original camera settings for restoration
//     private LayerMask originalCullingMask;
//     private CameraClearFlags originalClearFlags;

//     void Awake()
//     {
//         // Ensure required references are assigned
//         if (thirdPersonCamera == null)
//         {
//             Debug.LogError("Third Person Camera is not assigned in the Inspector.");
//         }

//         if (agentView == null)
//         {
//             Debug.LogError("Agent View (RawImage) is not assigned in the Inspector.");
//         }

//         if (radialMeterPanel == null)
//         {
//             Debug.LogError("Radial Meter Panel is not assigned in the Inspector.");
//         }

//         // Store the original camera settings
//         originalCullingMask = thirdPersonCamera.cullingMask;
//         originalClearFlags = thirdPersonCamera.clearFlags;
//     }

//     void Start()
//     {
//         // Cache the RectTransform component for efficiency
//         agentViewRectTransform = agentView.GetComponent<RectTransform>();

//         // Initialize the view to the "Y" pressed state (first-person view)
//         InitializeDefaultView();
//     }

//     void Update()
//     {
//         // Listen for the "Y" key press to toggle views
//         if (Input.GetKeyDown(switchKey))
//         {
//             ToggleView();
//         }
//     }

//     /// <summary>
//     /// Initializes the camera and UI to the default "Y" pressed state.
//     /// </summary>
//     void InitializeDefaultView()
//     {
//         isFirstPersonFullScreen = true; // Set the initial state to first-person view

//         // Expand the RawImage to cover the entire screen
//         agentViewRectTransform.anchorMin = new Vector2(0, 0);
//         agentViewRectTransform.anchorMax = new Vector2(1, 1);
//         agentViewRectTransform.offsetMin = Vector2.zero;
//         agentViewRectTransform.offsetMax = Vector2.zero;

//         // Adjust the third-person camera to render only UI
//         thirdPersonCamera.cullingMask = (1 << LayerMask.NameToLayer("UI")); // Render only UI layer
//         thirdPersonCamera.clearFlags = CameraClearFlags.Depth; // Maintain existing depth settings

//         // Activate the RadialMeterPanel
//         if (radialMeterPanel != null)
//         {
//             radialMeterPanel.SetActive(true);
//         }
//         thermoSensorGridPanel.SetActive(true);
//     }

//     /// <summary>
//     /// Toggles between first-person and third-person views when the switch key is pressed.
//     /// </summary>
//     void ToggleView()
//     {
//         // Toggle the view state
//         isFirstPersonFullScreen = !isFirstPersonFullScreen;

//         if (isFirstPersonFullScreen)
//         {
//             // Activate first-person view

//             // Expand the RawImage to cover the entire screen
//             agentViewRectTransform.anchorMin = new Vector2(0, 0);
//             agentViewRectTransform.anchorMax = new Vector2(1, 1);
//             agentViewRectTransform.offsetMin = Vector2.zero;
//             agentViewRectTransform.offsetMax = Vector2.zero;

//             // Adjust the third-person camera to render only UI
//             thirdPersonCamera.cullingMask = (1 << LayerMask.NameToLayer("UI")); // Render only UI layer
//             thirdPersonCamera.clearFlags = CameraClearFlags.Depth; // Maintain existing depth settings

//             // Activate the RadialMeterPanel
//             if (radialMeterPanel != null)
//             {
//                 radialMeterPanel.SetActive(true);
//             }
//             thermoSensorGridPanel.SetActive(true);
//         }
//         else
//         {
//             // Activate third-person view

//             // Restore the RawImage to its original size and position
//             // Example values; adjust based on your initial layout
//             agentViewRectTransform.anchorMin = new Vector2(0.8f, 0.1f); // Original anchorMin
//             agentViewRectTransform.anchorMax = new Vector2(0.95f, 0.3f); // Original anchorMax
//             agentViewRectTransform.offsetMin = Vector2.zero;
//             agentViewRectTransform.offsetMax = Vector2.zero;

//             // Restore the third-person camera's original culling mask and clear flags
//             thirdPersonCamera.cullingMask = originalCullingMask;
//             thirdPersonCamera.clearFlags = originalClearFlags;

//             // Deactivate the RadialMeterPanel
//             if (radialMeterPanel != null)
//             {
//                 radialMeterPanel.SetActive(false);
//             }
//             thermoSensorGridPanel.SetActive(false);
//         }
//     }
// }
