# ObstacleCollector Documentation

## Overview
The `ObstacleCollector` automates the collection and export of obstacle configuration data from the Unity scene in EVAAA. It scans the scene for obstacles, extracts their properties, and generates a structured JSON configuration file for reproducible environment setup and procedural generation.

---

## Usage (Beginner & Advanced)
- **Beginner**: Attach `ObstacleCollector` to a GameObject in your Unity scene. Configure options (output file name, filter, padding, decimal places) in the Inspector. Click the "Collect Obstacles" button in the Inspector to generate a JSON config of all obstacles in the scene.
- **Advanced**: Use filtering and formatting options to customize which obstacles are collected and how their data is exported. Integrate the generated JSON with procedural spawners or environment setup scripts for reproducible experiments.

---

## Config Reference Table
| Field              | Type    | Description                                         | Example/Config File                  |
|--------------------|---------|-----------------------------------------------------|--------------------------------------|
| outputFileName     | string  | Name of the generated JSON file                     | Inspector (default: generatedObstacleConfig.json) |
| prefabFolder       | string  | Folder containing obstacle prefabs                  | Inspector                           |
| obstacleNameFilter | string  | Filter string for selecting obstacles by name       | Inspector                           |
| defaultPadding     | float   | Default padding value for obstacles                 | Inspector (default: 2.0)             |
| decimalPlaces      | int     | Number of decimal places for exported values        | Inspector (default: 2)               |
| groups             | array   | List of obstacle groups in exported config          | obstacleConfig.json                  |
| prefabName         | string  | Name of the obstacle prefab                         | obstacleConfig.json                  |
| count              | int     | Number of obstacles in the group                    | obstacleConfig.json                  |
| temperature        | float   | Temperature value for the obstacle group            | obstacleConfig.json                  |
| padding            | float   | Padding for the obstacle group                      | obstacleConfig.json                  |
| position           | object  | Position range for the group                        | obstacleConfig.json                  |
| rotationRange      | object  | Rotation range for the group                        | obstacleConfig.json                  |
| scaleRange         | object  | Scale range for the group                           | obstacleConfig.json                  |

---

## Real Example Config
From `Config/exp-goal-manipulation-WaterToFood/obstacleConfig.json`:
```json
{
    "groups": [
        {
            "prefabName": "Tree",
            "count": 0,
            "temperature": 0.0,
            "padding": 2.0,
            "position": {
                "xMin": -45, "xMax": 45,
                "yMin": 0, "yMax": 0,
                "zMin": -45, "zMax": 45
            },
            "rotationRange": { "x": 0, "y": 0, "z": 0 },
            "scaleRange": {
                "xMin": 0.8, "xMax": 1.2,
                "yMin": 0.8, "yMax": 1.2,
                "zMin": 0.8, "zMax": 1.2
            }
        }
    ]
}
```

---

## Mapping Config Fields to Script Behavior
- **outputFileName**: Sets the name of the exported JSON file.
- **prefabFolder**: Used to locate obstacle prefabs (for reference, not required for export).
- **obstacleNameFilter**: Filters which obstacles in the scene are included in the export.
- **defaultPadding**: Sets the default padding value for all exported obstacles.
- **decimalPlaces**: Controls the precision of all exported float values.
- **groups**: Each group in the exported config corresponds to a set of obstacles with the same prefab name and properties.
- **position/rotationRange/scaleRange**: Define the spatial configuration for each obstacle group, used by procedural spawners.

---

## Main Script Methods
- `CollectObstacles()`: Scans the scene, collects obstacle data, and writes the configuration to a JSON file.
- `FormatFloat(float value)`: Formats float values to the specified decimal places.

---

## Practical Tips
- Use `obstacleNameFilter` to export only specific types of obstacles (e.g., only trees or rocks).
- Adjust `decimalPlaces` for more compact or more precise JSON output.
- The generated JSON can be used directly in procedural generation scripts for reproducible experiments.
- Review code comments in `Assets/Scripts/Utility/ObstacleCollector.cs` for advanced usage.

---

## Further Details
- ObstacleCollector is designed for easy integration with both manual and automated environment setup workflows.
- The exported config format matches what is expected by procedural spawners and environment generators in EVAAA. 