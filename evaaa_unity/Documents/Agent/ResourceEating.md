# ResourceEating.cs

## Overview
The `ResourceEating` component manages how agents in EVAAA interact with consumable resources such as food, water, and ponds. It detects when the agent is in range to consume resources, updates internal state variables, and handles resource respawn logic. Nearly all aspects of resource consumption are controlled through the agent's and environment's config files—making it easy for both beginners and advanced users to customize behavior without coding.

## How to Use
- **For beginners:**
  - You can control how the agent eats and how resources are placed by editing the relevant fields in `agentConfig.json` (e.g., `autoEat`, `eatingDistance`, `resourceFoodValue`, `resourceWaterValue`) and `resourceConfig.json` (e.g., resource types, count, position).
  - No coding is required—just open the files in a text editor, change values, and press Play in Unity.
- **For advanced users:**
  - You can extend or modify the resource eating logic by editing `ResourceEating.cs` in `Assets/Scripts/Agent/`.

## Configuration Reference
Below is a list of all relevant config fields for resource eating, with explanations and examples:

**From `agentConfig.json`:**
| Field | Type/Format | Example | Description |
|-------|-------------|---------|-------------|
| `autoEat` | bool | `false` | If true, the agent eats automatically when near a resource. |
| `eatingDistance` | float | `1.0` | How close the agent must be to eat a resource. |
| `resourceFoodValue` | float | `3.0` | How much food increases when eating. |
| `resourceWaterValue` | float | `3.0` | How much water increases when drinking. |

**From `resourceConfig.json` (per group):**
| Field | Type/Format | Example | Description |
|-------|-------------|---------|-------------|
| `prefabName` | string | `"Food"` | The prefab to use for the resource (e.g., Food, Water, Pond). |
| `prefabLabel` | string | `"Food"` | Label for the resource (for UI or logging). |
| `resourceType` | string | `"Random"` | How the resource is placed (Random, Static, etc.). |
| `count` | int | `50` | Number of resources to spawn. |
| `position` | object `{xMin, xMax, yMin, yMax, zMin, zMax}` | `{ "xMin": -40, "xMax": 40, ... }` | Range for randomizing resource spawn position. |
| `rotationRange` | object `{x, y, z}` | `{ "x": 0, "y": 360, "z": 0 }` | Initial rotation of the resource. |
| `scaleRange` | object `{xMin, xMax, yMin, yMax, zMin, zMax}` | `{ "xMin": 0.6, "xMax": 0.6, ... }` | Range for randomizing resource scale. |

## Example agentConfig.json (relevant fields)
```json
{
  "autoEat": false,
  "eatingDistance": 1.0,
  "resourceFoodValue": 3.0,
  "resourceWaterValue": 3.0
}
```

## Example resourceConfig.json (relevant fields)
```json
{
  "groups": [
    {
      "prefabName": "Food",
      "prefabLabel": "Food",
      "resourceType": "Random",
      "count": 50,
      "position": { "xMin": -40, "xMax": 40, "yMin": 1, "yMax": 1, "zMin": -40, "zMax": 40 },
      "rotationRange": { "x": 0, "y": 360, "z": 0 },
      "scaleRange": { "xMin": 0.6, "xMax": 0.6, "yMin": 0.6, "yMax": 0.6, "zMin": 0.6, "zMax": 0.6 }
    }
  ]
}
```

## Main Script Methods & How Config Maps to Behavior
- The agent loads all config fields at startup and uses them to set resource eating parameters.
- **Resource Detection:** The script detects when the agent is near a resource (food, water, pond) using trigger zones.
- **Consumption:** If `autoEat` is true or the agent takes an eat action, the resource is consumed and the agent's internal state is updated.
- **Resource Respawn:** When a resource is eaten, it is relocated (respawned) by the resource spawner.
- **Observations:** Resource consumption events can be logged for analysis.

---

For further details, see the code comments in `Assets/Scripts/Agent/ResourceEating.cs` or explore the config files in your experiment folder. 