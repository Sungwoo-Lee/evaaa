# Predator.cs

## Overview
The `Predator` is a dynamic agent in EVAAA that acts as a moving threat, challenging the main agent to develop adaptive and evasive behaviors. The predator uses a state machine (Resting, Searching, Chasing, Attacking), navigates with Unity's NavMesh, and interacts with the environment, landmarks, and day/night cycle. All key predator behaviors and parameters are controlled through easy-to-edit config files—no coding required for most users.

## How to Use
- **For beginners:**
  - You can control how predators move, see, and attack by editing the `predatorConfig.json` file in your chosen config folder (e.g., `Config/train-level-4.1-Predator/predatorConfig.json`).
  - Landmarks and the day/night cycle, which affect predator behavior, are configured in `landmarkConfig.json` and `daynightConfig.json`.
  - No coding is required—just edit the config files and press Play in Unity.
- **For advanced users:**
  - You can extend or modify predator logic by editing `Predator.cs` in `Assets/Scripts/Agent/`.

## Configuration Reference
Below is a list of all relevant config fields for predators, with explanations and examples. Each predator is defined in a `groups` array in `predatorConfig.json`.

| Field | Type/Format | Example | Description |
|-------|-------------|---------|-------------|
| `prefabName` | string | `"Predator"` | The prefab to use for the predator. |
| `count` | int | `1` | Number of predators to spawn in this group. |
| `position` | object `{xMin, xMax, yMin, yMax, zMin, zMax}` | `{ "xMin": -10.0, "xMax": -5.0, ... }` | Range for randomizing predator's spawn position. |
| `rotationRange` | object `{x, y, z}` | `{ "x": 0.0, "y": 0.0, "z": 0.0 }` | Initial rotation of the predator. |
| `scaleRange` | object `{xMin, xMax, yMin, yMax, zMin, zMax}` | `{ "xMin": 1.0, "xMax": 1.0, ... }` | Range for randomizing predator's scale. |
| `walkSpeed` | float | `6.0` | How fast the predator moves. |
| `turnSpeed` | float | `180.0` | How quickly the predator turns. |
| `viewAngle` | float | `120.0` | Predator's field of view (degrees). |
| `viewDistance` | float | `20.0` | How far the predator can see. |
| `damageAmount` | float | `5.0` | Damage dealt to the agent per attack. |
| `maxDamage` | float | `20.0` | Maximum damage predator can inflict in one episode. |
| `attackInterval` | float | `1.0` | Time (seconds) between attacks. |
| `maxRestingSteps` | int | `50` | Max steps predator spends resting. |
| `maxSearchingSteps` | int | `150` | Max steps predator spends searching. |
| `searchingActionInterval` | int | `60` | Steps between search actions. |

**Landmark Area (landmarkConfig.json):**
- `customPattern`, `patternRows`, `patternCols`: Define the landmark area shape for predator navigation.
- `landmarkRadius`, `overlapPadding`: Control landmark size and spacing.

**Day/Night Cycle (daynightConfig.json):**
- `enableDayNightCycle`: If true, predator behavior changes with time of day (e.g., resting at night).
- Other fields (e.g., `fogChangeSpeed`, `dayFogDensity`) affect environment but may also influence predator perception.

## Example predatorConfig.json
```json
{
  "groups": [
    {
      "prefabName": "Predator",
      "count": 1,
      "position": { "xMin": -10.0, "xMax": -5.0, "yMin": 0.0, "yMax": 0.0, "zMin": -10.0, "zMax": -5.0 },
      "rotationRange": { "x": 0.0, "y": 0.0, "z": 0.0 },
      "scaleRange": { "xMin": 1.0, "xMax": 1.0, "yMin": 1.0, "yMax": 1.0, "zMin": 1.0, "zMax": 1.0 },
      "walkSpeed": 6.0,
      "turnSpeed": 180.0,
      "viewAngle": 120.0,
      "viewDistance": 20.0,
      "damageAmount": 5.0,
      "maxDamage": 20.0,
      "attackInterval": 1.0,
      "maxRestingSteps": 50,
      "maxSearchingSteps": 150,
      "searchingActionInterval": 60
    }
  ]
}
```

## Main Script Methods & How Config Maps to Behavior
- The predator loads all config fields at startup and uses them to set movement, perception, and attack parameters.
- **Initialization:** `InitializePredator()` and `InitializeNavMesh()` set up the predator and its navigation.
- **State Machine:** Predator switches between Resting, Searching, Chasing, and Attacking based on environment and agent proximity.
- **Perception:** `viewAngle` and `viewDistance` control how the predator detects agents.
- **Attack:** `damageAmount`, `maxDamage`, and `attackInterval` control how and when the predator attacks.
- **Landmarks:** The predator uses landmark area logic to determine valid movement zones.
- **Day/Night:** The predator rests at night if `enableDayNightCycle` is true.

---

For further details, see the code comments in `Assets/Scripts/Agent/Predator.cs` or explore the config files in your experiment folder. 