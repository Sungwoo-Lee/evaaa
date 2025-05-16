using UnityEngine;
using Unity.MLAgents.Sensors;

public class ConfigurableCameraSensor : CameraSensorComponent
{
    [Header("Camera Sensor Configuration")]
    [Tooltip("The camera used for observations. If not set, will try to find one on this GameObject or create one.")]
    public new Camera camera;
    
    [Tooltip("Width of the camera observation")]
    public int observationWidth = 84;
    
    [Tooltip("Height of the camera observation")]
    public int observationHeight = 84;
    
    [Tooltip("Whether to convert the observation to grayscale")]
    public bool grayscale = false;
    
    [Tooltip("Compression type for the camera observation")]
    public SensorCompressionType compressionType = SensorCompressionType.PNG;

    [Tooltip("Name of the sensor")]
    public string sensorName = "CameraSensor";

    private CameraSensor sensor;
    private bool isInitialized = false;

    private void Awake()
    {
        InitializeCamera();
    }

    private void OnEnable()
    {
        InitializeCamera();
    }

    private void InitializeCamera()
    {
        if (isInitialized) return;

        // If no camera is assigned, try to get one from this GameObject
        if (camera == null)
        {
            camera = GetComponent<Camera>();
            
            // If still no camera, try to find one in children
            if (camera == null)
            {
                camera = GetComponentInChildren<Camera>();
            }

            // If still no camera, create one
            if (camera == null)
            {
                var cameraObj = new GameObject("AgentCamera");
                cameraObj.transform.parent = transform;
                cameraObj.transform.localPosition = new Vector3(0, 0, 0);
                cameraObj.transform.localRotation = Quaternion.identity;
                
                camera = cameraObj.AddComponent<Camera>();
                camera.enabled = true;
                camera.orthographic = false;
                camera.depth = 0;
            }
        }

        isInitialized = true;
    }

    public override ISensor[] CreateSensors()
    {
        InitializeCamera();

        if (camera == null)
        {
            Debug.LogError($"Camera is not set on {gameObject.name}'s ConfigurableCameraSensor component. Please assign a camera.");
            return new ISensor[0];
        }

        sensor = new CameraSensor(
            camera,
            observationWidth,
            observationHeight,
            grayscale,
            sensorName,
            compressionType
        );
        return new ISensor[] { sensor };
    }

    // Method to update sensor dimensions at runtime
    public void UpdateSensorDimensions(int width, int height)
    {
        if (sensor != null && camera != null)
        {
            observationWidth = width;
            observationHeight = height;
            sensor = new CameraSensor(
                camera,
                observationWidth,
                observationHeight,
                grayscale,
                sensorName,
                compressionType
            );
        }
    }

    // Method to update grayscale setting at runtime
    public void UpdateGrayscale(bool isGrayscale)
    {
        if (sensor != null && camera != null)
        {
            grayscale = isGrayscale;
            sensor = new CameraSensor(
                camera,
                observationWidth,
                observationHeight,
                grayscale,
                sensorName,
                compressionType
            );
        }
    }
}
