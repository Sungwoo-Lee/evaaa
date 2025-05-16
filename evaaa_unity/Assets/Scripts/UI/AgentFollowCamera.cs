using UnityEngine;
using System.IO;
using Assets.Scripts.Utility;

[System.Serializable]
public class CameraConfig
{
    public ThreeDVector initCameraPosition;
    public ThreeDVector initCameraAngle;
}

public class AgentFollowCamera : MonoBehaviour
{
    public string configFileName = "cameraConfig.json";
    public GameObject agent;

    private CameraConfig cameraConfig; // Camera configuration data
    private ConfigLoader configLoader; // Reference to ConfigLoader

    public void InitializeCamera(ConfigLoader loader)
    {
        configLoader = loader;
        if (configLoader == null)
        {
            Debug.LogError("ConfigLoader is not set. Ensure ConfigLoader is initialized.");
            return;
        }

        LoadConfig();

        if (cameraConfig == null)
        {
            Debug.LogError("Camera configuration is not loaded. Call ReloadConfig() before InitializeCamera().");
            return;
        }

        transform.rotation = Quaternion.Euler(cameraConfig.initCameraAngle.ToVector3());
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

        cameraConfig = configLoader.LoadConfig<CameraConfig>(configFileName);

        if (cameraConfig == null)
        {
            Debug.LogError("Invalid camera configuration.");
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cameraConfig != null && agent != null)
        {
            transform.position = agent.transform.position + cameraConfig.initCameraPosition.ToVector3();
        }
    }
}
