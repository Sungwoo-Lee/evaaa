# ResourceSpawner.cs

## Overview
The `ResourceSpawner` is the component that places resources (food, water, ponds, etc.) in the EVAAA environment. It supports static, random, and grouped random resource distributions, enabling research on foraging, homeostasis, and multi-agent dynamics. Nearly all aspects of resource placement and properties are controlled through a simple JSON config file—making it easy for both beginners and advanced users to customize experiments without coding.

## How to Use
- **For beginners:**
  - You can change the number, type, and distribution of resources by editing the `resourceConfig.json` file in your chosen config folder (e.g., `Config/exp-Ymaze/resourceConfig.json`).
  - No coding is required—just open the file in a text editor, change values, and press Play in Unity.
- **For advanced users:**
  - You can extend or modify the resource logic by editing `ResourceSpawner.cs` in `Assets/Scripts/Environment/`.

## Configuration Reference
Below is a complete list of all config fields in `resourceConfig.json`, with types, examples, and clear descriptions. Resources are defined in groups:

| Field         | Type/Format | Example | Description |
|--------------|-------------|---------|-------------|
| `prefabName` | string      | `"Food"` | Name of the resource prefab (must exist in `Resources/Resources`). |
| `prefabLabel`| string      | `"Food"` | Semantic label for the resource (used for learning/categorization). |
| `resourceType`| string     | `"Random"` | Distribution strategy: `Static`, `Random`, or `GroupedRandom`. |
| `count`      | int         | `3`     | Number of resources in this group. |
| `position`   | object `{xMin,xMax,yMin,yMax,zMin,zMax}` | `{ "xMin": -16, "xMax": -20, ... }` | Placement bounds for each resource. |
| `rotationRange` | object `{x,y,z}` | `{ "x": 0, "y": 0, "z": 0 }` | Range of possible rotations for each resource. |
| `scaleRange` | object `{xMin,xMax,yMin,yMax,zMin,zMax}` | `{ "xMin": 0.6, "xMax": 0.6, ... }` | Range of possible scales for each resource. |

## Example resourceConfig.json (from exp-Ymaze)
```json
{
    "groups": [
        {
            "prefabName": "Food",
            "prefabLabel": "Food",
            "resourceType": "Random",
            "count": 3,
            "position": {
                "xMin": -16, "xMax": -20,
                "yMin": 0.6, "yMax": 0.6,
                "zMin": 24, "zMax": 28
            },
            "rotationRange": {
                "x": 0, "y": 0, "z": 0
            },
            "scaleRange": {
                "xMin": 0.6, "xMax": 0.6,
                "yMin": 0.6, "yMax": 0.6,
                "zMin": 0.6, "zMax": 0.6
            }
        },
        {
            "prefabName": "Water",
            "prefabLabel": "Water",
            "resourceType": "Random",
            "count": 3,
            "position": {
                "xMin": 16, "xMax": 20,
                "yMin": 0.6, "yMax": 0.6,
                "zMin": 24, "zMax": 28
            },
            "rotationRange": {
                "x": 0, "y": 0, "z": 0
            },
            "scaleRange": {
                "xMin": 0.6, "xMax": 0.6,
                "yMin": 0.6, "yMax": 0.6,
                "zMin": 0.6, "zMax": 0.6
            }
        },
        {
            "prefabName": "Pond",
            "prefabLabel": "Pond",
            "resourceType": "Static",
            "count": 0,
            "position": {
                "xMin": -40, "xMax": -40,
                "yMin": 1, "yMax": 1,
                "zMin": -40, "zMax": -40
            },
            "rotationRange": {
                "x": 0, "y": 0, "z": 0
            },
            "scaleRange": {
                "xMin": 1.9, "xMax": 1.9,
                "yMin": 1, "yMax": 1,
                "zMin": 1.7, "zMax": 1.7
            }
        }
    ]
}
```

## Main Script Methods & How Config Maps to Behavior
- The resource config is loaded at runtime using the `ConfigLoader` utility, with the file specified by `configFileName` (default: `resourceConfig.json`).
- **Initialization:** `InitializeResourceSpawner()` and `InitializeResources()` read the config and set up all resource groups.
- **Distribution:** Each group spawns the specified number of resources, with randomization within the provided position, rotation, and scale ranges, according to the `resourceType`.
- **Labels:** `prefabLabel` is used for semantic learning and agent observations.
- **Prefab Loading:** Prefabs must exist in `Resources/Resources` and match the `prefabName` field.

## Practical Tips for Research & Tuning
- **Resource Abundance:** Increase `count` and use wide spatial bounds for abundant environments; decrease for scarcity.
- **Distribution Strategy:** Use `Static` for fixed locations, `Random` for scattered, and `GroupedRandom` for clustered resources.
- **Reproducibility:** Always save and document your config files for each experiment.
- **Debugging:** Use distinct prefabs and labels to visually distinguish resource types.
- **Multi-Agent:** Use low `count` and overlapping bounds to induce competition; use high `count` for cooperation studies.

## Further Details
See the code in `Assets/Scripts/Environment/ResourceSpawner.cs` for implementation details, or experiment with different configs in the `Config/` folders. All fields are documented above for easy mapping between config, code, and experiment design. 