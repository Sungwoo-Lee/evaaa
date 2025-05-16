using System.IO;
using UnityEngine;

public class ConfigLoader : MonoBehaviour
{
    [Header("Configuration")]
    public string mainConfigFileName = "mainConfig.json"; // Public variable for main config file name

    public MainConfig mainConfig;
    public string configFolderPath;

    public void InitializeConfigLoader()
    {
        LoadMainConfig();
    }

    private void LoadMainConfig()
    {
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        // Go up four levels from Data: Data -> Resources -> Contents -> build.app -> parent folder
        string appRoot = Directory.GetParent(Application.dataPath).Parent.FullName;
        string mainConfigPath = Path.Combine(appRoot, "Config", mainConfigFileName);
#else
        string mainConfigPath = Path.Combine(Application.dataPath, "..", "Config", mainConfigFileName);
#endif
        if (!File.Exists(mainConfigPath))
        {
            Debug.LogError($"Main config file not found: {mainConfigPath}");
            return;
        }
        string jsonContent = File.ReadAllText(mainConfigPath);
        mainConfig = JsonUtility.FromJson<MainConfig>(jsonContent);
        if (mainConfig == null || string.IsNullOrEmpty(mainConfig.configFolderName))
        {
            Debug.LogError("Invalid main config file.");
            return;
        }
        SetConfigFolder(mainConfig.configFolderName);
        Debug.Log($"ConfigLoader: SetConfigFolder as {configFolderPath}.");
    }

    private void SetConfigFolder(string folderName)
    {
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        string appRoot = Directory.GetParent(Application.dataPath).Parent.FullName;
        configFolderPath = Path.Combine(appRoot, "Config", folderName);
#else
        configFolderPath = Path.Combine(Application.dataPath, "..", "Config", folderName);
#endif
        if (!Directory.Exists(configFolderPath))
        {
            Debug.LogError($"Config folder not found: {configFolderPath}");
        }
    }

    public T LoadConfig<T>(string configFileName) where T : new()
    {
        if (string.IsNullOrEmpty(configFolderPath))
        {
            Debug.LogError("Config folder is not set. Call SetConfigFolder() first.");
            return new T();
        }

        string configFilePath = Path.Combine(configFolderPath, configFileName);

        if (!File.Exists(configFilePath))
        {
            Debug.LogError($"Config file not found: {configFilePath}");
            return new T();
        }

        string jsonContent = File.ReadAllText(configFilePath);
        T config = JsonUtility.FromJson<T>(jsonContent);
        return config;
    }

    // New method to get the full path of a configuration file
    public string GetFullPath(string configFileName)
    {
        if (string.IsNullOrEmpty(configFolderPath))
        {
            Debug.LogError("Config folder is not set. Call SetConfigFolder() first.");
            return string.Empty;
        }

        return Path.Combine(configFolderPath, configFileName);
    }
}

[System.Serializable]
public class MainConfig
{
    public bool isAIControlled;
    public string configFolderName;
    public RecordingScreen recordingScreen;
    public ExperimentData experimentData;
}

[System.Serializable]
public class RecordingScreen
{
    public string recordingFolderName;
    public bool recordEnable;
}

[System.Serializable]
public class ExperimentData
{
    public string baseFolderName;
    public string fileNamePrefix;
    public bool recordEnable;
}
