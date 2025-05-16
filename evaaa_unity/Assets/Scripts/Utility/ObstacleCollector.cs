using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System;
using Newtonsoft.Json;

[System.Serializable]
public class ObstaclePositionRange
{
    public float xMin, xMax;
    public float yMin, yMax;
    public float zMin, zMax;
}

[System.Serializable]
public class ObstacleRotationRange
{
    public float x, y, z;
}

[System.Serializable]
public class ObstacleScaleRange
{
    public float xMin, xMax;
    public float yMin, yMax;
    public float zMin, zMax;
}

[System.Serializable]
public class CollectedObstacleGroup
{
    public string prefabName;
    public int count;
    public float temperature;
    public float padding;
    public ObstaclePositionRange position;
    public ObstacleRotationRange rotationRange;
    public ObstacleScaleRange scaleRange;
}

[System.Serializable]
public class CollectedObstacleConfig
{
    public List<CollectedObstacleGroup> groups = new List<CollectedObstacleGroup>();
}

public class ObstacleCollector : MonoBehaviour
{
    public string outputFileName = "generatedObstacleConfig.json";
    public string prefabFolder = "Obstacles";
    public string obstacleNameFilter = ""; // New field for obstacle name filter
    public float defaultPadding = 2.0f; // Default padding value
    [Range(0, 6)]
    public int decimalPlaces = 2; // Number of decimal places to maintain

    private string FormatFloat(float value)
    {
        return value.ToString($"F{decimalPlaces}");
    }

    public void CollectObstacles()
    {
        CollectedObstacleConfig config = new CollectedObstacleConfig();
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (string.IsNullOrEmpty(obstacleNameFilter) || obj.name.Contains(obstacleNameFilter))
            {
                string prefabName = Regex.Replace(obj.name, @"\(Clone\).*", "").Trim();

                CollectedObstacleGroup group = new CollectedObstacleGroup
                {
                    prefabName = prefabName,
                    count = 1,
                    temperature = float.Parse(FormatFloat(0.0f)),
                    padding = float.Parse(FormatFloat(defaultPadding)),
                    position = new ObstaclePositionRange
                    {
                        xMin = float.Parse(FormatFloat(obj.transform.localPosition.x)),
                        xMax = float.Parse(FormatFloat(obj.transform.localPosition.x)),
                        yMin = float.Parse(FormatFloat(obj.transform.localPosition.y)),
                        yMax = float.Parse(FormatFloat(obj.transform.localPosition.y)),
                        zMin = float.Parse(FormatFloat(obj.transform.localPosition.z)),
                        zMax = float.Parse(FormatFloat(obj.transform.localPosition.z))
                    },
                    rotationRange = new ObstacleRotationRange
                    {
                        x = float.Parse(FormatFloat(obj.transform.localEulerAngles.x)),
                        y = float.Parse(FormatFloat(obj.transform.localEulerAngles.y)),
                        z = float.Parse(FormatFloat(obj.transform.localEulerAngles.z))
                    },
                    scaleRange = new ObstacleScaleRange
                    {
                        xMin = float.Parse(FormatFloat(obj.transform.localScale.x)),
                        xMax = float.Parse(FormatFloat(obj.transform.localScale.x)),
                        yMin = float.Parse(FormatFloat(obj.transform.localScale.y)),
                        yMax = float.Parse(FormatFloat(obj.transform.localScale.y)),
                        zMin = float.Parse(FormatFloat(obj.transform.localScale.z)),
                        zMax = float.Parse(FormatFloat(obj.transform.localScale.z))
                    }
                };
                config.groups.Add(group);
            }
        }

        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            FloatFormatHandling = FloatFormatHandling.String
        };

        string json = JsonConvert.SerializeObject(config, settings);
        string outputPath = Path.Combine(Application.dataPath, outputFileName);

        // Post-process: collapse position, rotationRange, and scaleRange objects to single lines
        // Matches objects like { "xMin": ..., "xMax": ..., "yMin": ..., ... "zMax": ... }
        json = Regex.Replace(json, @"(\{\s*""xMin"": [^}]+?""zMax"": [^}]+?\})", m => Regex.Replace(m.Value.Replace("\n", "").Replace("\r", ""), @"\s+", " "), RegexOptions.Multiline);
        // Matches objects like { "x": ..., "y": ..., "z": ... }
        json = Regex.Replace(json, @"(\{\s*""x"": [^}]+?""z"": [^}]+?\})", m => Regex.Replace(m.Value.Replace("\n", "").Replace("\r", ""), @"\s+", " "), RegexOptions.Multiline);

        File.WriteAllText(outputPath, json);
        Debug.Log($"Obstacle configuration saved to {outputPath}");
    }
}