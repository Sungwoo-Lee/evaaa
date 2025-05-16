# ObstacleSpawner.cs

## Overview
The `ObstacleSpawner` is the component that places obstacles in the EVAAA environment. It supports complex, parameterized obstacle groups with thermal properties, enabling research on navigation, path planning, and thermoregulation. Nearly all aspects of obstacle placement and properties are controlled through a simple JSON config file—making it easy for both beginners and advanced users to customize experiments without coding.

## How to Use
- **For beginners:**
  - You can change the number, type, and properties of obstacles by editing the `obstacleConfig.json` file in your chosen config folder (e.g., `Config/exp-Ymaze/obstacleConfig.json`).
  - No coding is required—just open the file in a text editor, change values, and press Play in Unity.
- **For advanced users:**
  - You can extend or modify the obstacle logic by editing `ObstacleSpawner.cs` in `Assets/Scripts/Environment/`.

## Configuration Reference
Below is a complete list of all config fields in `obstacleConfig.json`, with types, examples, and clear descriptions. Obstacles are defined in groups:

| Field         | Type/Format | Example | Description |
|--------------|-------------|---------|-------------|
| `prefabName` | string      | `"Block"` | Name of the obstacle prefab (must exist in `Resources/Obstacles`). |
| `count`      | int         | `1`     | Number of obstacles in this group. |
| `temperature`| float       | `0.0`   | Thermal property of the obstacle (affects local temperature). |
| `padding`    | float       | `0.0`   | Minimum distance between obstacles (prevents overlap). |
| `position`   | object `{xMin,xMax,yMin,yMax,zMin,zMax}` | `{ "xMin": -10.2, "xMax": -10.2, ... }` | Placement bounds for each obstacle. |
| `rotationRange` | object `{x,y,z}` | `{ "x": 0.0, "y": 320.0, "z": 0.0 }` | Range of possible rotations for each obstacle. |
| `scaleRange` | object `{xMin,xMax,yMin,yMax,zMin,zMax}` | `{ "xMin": 1.0, "xMax": 1.0, ... }` | Range of possible scales for each obstacle. |

## Example obstacleConfig.json (from exp-Ymaze)
```json
{
    "groups": [
      {
        "prefabName": "Block",
        "count": 1,
        "temperature": 0.0,
        "padding": 0.0,
        "position": {
          "xMin": -10.2,
          "xMax": -10.2,
          "yMin": 2.0,
          "yMax": 2.0,
          "zMin": 23.8,
          "zMax": 23.8
        },
        "rotationRange": {
          "x": 0.0,
          "y": 320.0,
          "z": 0.0
        },
        "scaleRange": {
          "xMin": 1.0,
          "xMax": 1.0,
          "yMin": 5.0,
          "yMax": 5.0,
          "zMin": 32.0,
          "zMax": 32.0
        }
      }
      // ... more groups ...
    ]
}
```

## Main Script Methods & How Config Maps to Behavior
- The obstacle config is loaded at runtime using the `ConfigLoader` utility, with the file specified by `configFileName` (default: `obstacleConfig.json`).
- **Initialization:** `InitializeObstacleSpawner()` reads the config and sets up all obstacle groups.
- **Placement:** Each group spawns the specified number of obstacles, with randomization within the provided position, rotation, and scale ranges.
- **Thermal Properties:** Each obstacle is assigned its `temperature` value, which can affect the thermal grid if integrated.
- **Padding:** The script uses the `padding` value to prevent overlap between obstacles.
- **Prefab Loading:** Prefabs must exist in `Resources/Obstacles` and match the `prefabName` field.

## Practical Tips for Research & Tuning
- **Navigation Difficulty:** Increase `count` or decrease `padding` for more challenging navigation.
- **Thermal Complexity:** Use varied `temperature` values to create hot/cold spots in the environment.
- **Reproducibility:** Always save and document your config files for each experiment.
- **Debugging:** Use distinct prefabs and scales to visually distinguish obstacle types.
- **Static vs. Random:** Obstacles with fixed positions (xMin==xMax, zMin==zMax) are treated as static; others are randomized each run.

## Further Details
See the code in `Assets/Scripts/Environment/ObstacleSpawner.cs` for implementation details, or experiment with different configs in the `Config/` folders. All fields are documented above for easy mapping between config, code, and experiment design. 