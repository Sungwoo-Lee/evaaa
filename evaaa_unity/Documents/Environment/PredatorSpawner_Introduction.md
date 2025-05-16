# PredatorSpawner.cs

## Overview
The `PredatorSpawner` is the component that places adversarial predator agents in the EVAAA environment. It supports parameterized predator groups with configurable movement, sensory, and attack properties, enabling research on avoidance, risk, and emergent multi-agent dynamics. Nearly all aspects of predator placement and behavior are controlled through a simple JSON config file—making it easy for both beginners and advanced users to customize experiments without coding.

## How to Use
- **For beginners:**
  - You can change the number, type, and properties of predators by editing the `predatorConfig.json` file in your chosen config folder (e.g., `Config/exp-Ymaze/predatorConfig.json`).
  - No coding is required—just open the file in a text editor, change values, and press Play in Unity.
- **For advanced users:**
  - You can extend or modify the predator logic by editing `PredatorSpawner.cs` in `Assets/Scripts/Environment/`.

## Configuration Reference
Below is a complete list of all config fields in `predatorConfig.json`, with types, examples, and clear descriptions. Predators are defined in groups:

| Field         | Type/Format | Example | Description |
|--------------|-------------|---------|-------------|
| `prefabName` | string      | `"Predator"` | Name of the predator prefab (must exist in `Resources/Predators`). |
| `count`      | int         | `0`     | Number of predators in this group. |
| `position`   | object `{xMin,xMax,yMin,yMax,zMin,zMax}` | `{ "xMin": -10.0, "xMax": -5.0, ... }` | Placement bounds for each predator. |
| `rotationRange` | object `{x,y,z}` | `{ "x": 0.0, "y": 0.0, "z": 0.0 }` | Range of possible rotations for each predator. |
| `scaleRange` | object `{xMin,xMax,yMin,yMax,zMin,zMax}` | `{ "xMin": 1.0, "xMax": 1.0, ... }` | Range of possible scales for each predator. |
| `walkSpeed`  | float       | `3.0`   | Movement speed of the predator. |
| `turnSpeed`  | float       | `180.0` | Turning speed of the predator. |
| `viewAngle`  | float       | `120.0` | Field of view (degrees). |
| `viewDistance`| float      | `10.0`  | Maximum detection range. |
| `damageAmount`| float      | `1.0`   | Damage per attack. |
| `maxDamage`  | float       | `5.0`   | Maximum damage capacity. |
| `attackInterval`| float    | `1.0`   | Time between attacks (seconds). |
| `maxRestingSteps`| int     | `50`    | Max duration in rest state (steps). |
| `maxSearchingSteps`| int   | `150`   | Max duration in search state (steps). |
| `searchingActionInterval`| int | `60` | Steps between search actions. |
| `padding`    | float       | `2.0`   | Minimum distance between predators. |

## Example predatorConfig.json (from exp-Ymaze)
```json
{
    "groups": [
        {
            "prefabName": "Predator",
            "count": 0,
            "position": {
                "xMin": -10.0,
                "xMax": -5.0,
                "yMin": 0.0,
                "yMax": 0.0,
                "zMin": -10.0,
                "zMax": -5.0
            },
            "rotationRange": {
                "x": 0.0,
                "y": 0.0,
                "z": 0.0
            },
            "scaleRange": {
                "xMin": 1.0,
                "xMax": 1.0,
                "yMin": 1.0,
                "yMax": 1.0,
                "zMin": 1.0,
                "zMax": 1.0
            },
            "walkSpeed": 3.0,
            "turnSpeed": 180.0,
            "viewAngle": 120.0,
            "viewDistance": 10.0,
            "damageAmount": 1.0,
            "maxDamage": 5.0,
            "attackInterval": 1.0,
            "maxRestingSteps": 50,
            "maxSearchingSteps": 150,
            "searchingActionInterval": 60
        }
    ]
} 
```

## Main Script Methods & How Config Maps to Behavior
- The predator config is loaded at runtime using the `ConfigLoader` utility, with the file specified by `configFileName` (default: `predatorConfig.json`).
- **Initialization:** `InitializePredatorSpawner()` reads the config and sets up all predator groups.
- **Placement:** Each group spawns the specified number of predators, with randomization within the provided position, rotation, and scale ranges.
- **Behavior:** Each predator is assigned its movement, sensory, and attack parameters from the config fields.
- **Prefab Loading:** Prefabs must exist in `Resources/Predators` and match the `prefabName` field.

## Practical Tips for Research & Tuning
- **Risk Level:** Increase `count`, `walkSpeed`, and `viewDistance` for more challenging environments.
- **Behavioral Complexity:** Use varied `maxRestingSteps`, `maxSearchingSteps`, and `attackInterval` for dynamic predator behavior.
- **Reproducibility:** Always save and document your config files for each experiment.
- **Debugging:** Use distinct prefabs and scales to visually distinguish predator types.
- **Spacing:** Adjust `padding` to prevent predator clustering.

## Further Details
See the code in `Assets/Scripts/Environment/PredatorSpawner.cs` for implementation details, or experiment with different configs in the `Config/` folders. All fields are documented above for easy mapping between config, code, and experiment design. 