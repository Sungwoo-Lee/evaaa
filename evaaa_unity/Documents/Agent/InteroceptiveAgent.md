# InteroceptiveAgent.cs

## Overview
The `InteroceptiveAgent` is the core agent in EVAAA, designed to model an embodied RL agent with internal physiological variables (essential variables: food, water, thermal, health) and rich, animal-like perception. Nearly all aspects of the agent's behavior, sensing, and internal state dynamics are controlled through a simple JSON config file—making it easy for both beginners and advanced users to customize experiments without coding.

## How to Use
- **For beginners:**
  - You can change how the agent moves, senses, and survives by editing the `agentConfig.json` file in your chosen config folder (e.g., `Config/train-level-1.1-ScatteredResource/agentConfig.json`).
  - No coding is required—just open the file in a text editor, change values, and press Play in Unity.
- **For advanced users:**
  - You can extend or modify the agent's logic by editing `InteroceptiveAgent.cs` in `Assets/Scripts/Agent/`.

## Configuration Reference
Below is a complete list of all config fields in `agentConfig.json`, with explanations and examples:

| Field | Type/Format | Example | Description |
|-------|-------------|---------|-------------|
| `singleTrial` | bool | `false` | If true, the agent runs only one episode per simulation. |
| `initRandomAgentPosition` | bool | `true` | If true, agent starts at a random position within the specified range. |
| `initAgentPosition` | object `{x, y, z}` | `{ "x": 0.0, "y": 1.0, "z": 0.0 }` | The agent's starting position if not random. |
| `initAgentAngle` | object `{x, y, z}` | `{ "x": 0.0, "y": 0.0, "z": 0.0 }` | The agent's starting rotation (Euler angles). |
| `randomPositionRange` | object `{xMin, xMax, yMin, yMax, zMin, zMax}` | `{ "xMin": -50.0, "xMax": 50.0, ... }` | Range for randomizing agent's start position. |
| `moveSpeed` | float | `20.0` | How fast the agent moves forward. |
| `turnSpeed` | float | `1200.0` | How quickly the agent turns left/right. |
| `autoEat` | bool | `false` | If true, the agent eats automatically when near food. |
| `eatingDistance` | float | `1.0` | How close the agent must be to eat a resource. |
| `rewardWindowSize` | int | `100` | Number of steps for moving average reward calculation. |
| `averageReward` | float | `0` | (Usually left at 0) Used internally for reward tracking. |
| `currentReward` | float | `0` | (Usually left at 0) Used internally for reward tracking. |
| `countEV` | int | `4` | Number of essential variables (usually 4: food, water, thermal, health). |
| `foodLevelRange` | object `{min, max}` | `{ "min": -15.0, "max": 15.0 }` | Allowed range for food level. |
| `resourceFoodValue` | float | `3.0` | How much food increases when eating. |
| `startFoodLevel` | float | `0.0` | Initial food level at episode start. |
| `waterLevelRange` | object `{min, max}` | `{ "min": -15.0, "max": 15.0 }` | Allowed range for water level. |
| `resourceWaterValue` | float | `3.0` | How much water increases when drinking. |
| `startWaterLevel` | float | `0.0` | Initial water level at episode start. |
| `thermoLevelRange` | object `{min, max}` | `{ "min": -15.0, "max": 15.0 }` | Allowed range for body temperature. |
| `startThermoLevel` | float | `0.0` | Initial body temperature at episode start. |
| `healthLevelRange` | object `{min, max}` | `{ "min": 0.0, "max": 100.0 }` | Allowed range for health. |
| `startHealthLevel` | float | `100.0` | Initial health at episode start. |
| `useTouchObs` | bool | `true` | If true, agent senses touch/collision. |
| `useCollisionObs` | bool | `true` | If true, agent receives collision sensor input. |
| `useOlfactoryObs` | bool | `true` | If true, agent receives olfactory (smell) input. |
| `olfactorySensorLength` | float | `100.0` | How far the agent can smell resources. |
| `useThermalObs` | bool | `true` | If true, agent receives thermal sensor input. |
| `relativeThermalObs` | bool | `false` | If true, thermal input is relative to agent's body temperature. |
| `foodCoefficient` | object | `{ "change_0": -0.045, ... }` | Coefficients for food level dynamics (decay, effects, etc.). |
| `waterCoefficient` | object | `{ "change_0": -0.03, ... }` | Coefficients for water level dynamics. |
| `thermoCoefficient` | object | `{ "change_0": 0.02, ... }` | Coefficients for thermal level dynamics. |
| `healthCoefficient` | object | `{ "change_0": 0.005, ... }` | Coefficients for health level dynamics. |
| `raysPerDirection` | int | `100` | Number of rays for collision detection. |
| `maxDistance` | float | `1.5` | Max distance for collision rays. |
| `radialRange` | float | `360.0` | Field of view for collision rays (degrees). |
| `damageConstant` | float | `0.0002` | How much damage is taken per collision. |
| `maxSteps` | int | `0` | Maximum steps per episode (0 = unlimited). |

## Example agentConfig.json
```json
{
  "singleTrial": false,
  "initRandomAgentPosition": true,
  "initAgentPosition": { "x": 0.0, "y": 1.0, "z": 0.0 },
  "initAgentAngle": { "x": 0.0, "y": 0.0, "z": 0.0 },
  "randomPositionRange": {
    "xMin": -50.0, "xMax": 50.0,
    "yMin": 1.0, "yMax": 1.0,
    "zMin": -50.0, "zMax": 50.0
  },
  "moveSpeed": 20.0,
  "turnSpeed": 1200.0,
  "autoEat": false,
  "eatingDistance": 1.0,
  "rewardWindowSize": 100,
  "averageReward": 0,
  "currentReward": 0,
  "countEV": 4,
  "foodLevelRange": { "min": -15.0, "max": 15.0 },
  "resourceFoodValue": 3.0,
  "startFoodLevel": 0.0,
  "waterLevelRange": { "min": -15.0, "max": 15.0 },
  "resourceWaterValue": 3.0,
  "startWaterLevel": 0.0,
  "thermoLevelRange": { "min": -15.0, "max": 15.0 },
  "startThermoLevel": 0.0,
  "healthLevelRange": { "min": 0.0, "max": 100.0 },
  "startHealthLevel": 100.0,
  "useTouchObs": true,
  "useCollisionObs": true,
  "useOlfactoryObs": true,
  "olfactorySensorLength": 100.0,
  "useThermalObs": true,
  "relativeThermalObs": false,
  "foodCoefficient": {
    "change_0": -0.045,
    "change_1": 0.0,
    "change_2": 0.0,
    "change_3": 0.0,
    "change_4": 0.0,
    "change_5": 0.0
  },
  "waterCoefficient": {
    "change_0": -0.03,
    "change_1": 0.0,
    "change_2": 0.0,
    "change_3": 0.0,
    "change_4": 0.0,
    "change_5": 0.0
  },
  "thermoCoefficient": {
    "change_0": 0.02,
    "change_1": 0.0,
    "change_2": 0.0,
    "change_3": 0.0,
    "change_4": 0.0
  },
  "healthCoefficient": {
    "change_0": 0.005,
    "change_1": 0.0,
    "change_2": 0.0,
    "change_3": 0.0,
    "change_4": 0.0,
    "change_5": 0.0
  },
  "raysPerDirection": 100,
  "maxDistance": 1.5,
  "radialRange": 360.0,
  "damageConstant": 0.0002,
  "maxSteps": 0
}
```

## Main Script Methods & How Config Maps to Behavior
- The agent loads all config fields at startup and uses them to set movement, sensing, and internal state parameters.
- **Initialization:** `InitializeAgent()` reads the config and sets up the agent.
- **Observations:** Sensor settings (touch, olfaction, thermal, collision) are enabled/disabled based on config fields.
- **Actions:** Movement and eating are controlled by `moveSpeed`, `turnSpeed`, `eatingDistance`, and `autoEat`.
- **Internal State:** Essential variable ranges, starting values, and coefficients control how food, water, thermal, and health change over time and with actions.
- **Reward:** The agent's reward is calculated from changes in internal variables, as set by the config.
---

For further details, see the code comments in `Assets/Scripts/Agent/InteroceptiveAgent.cs` or explore the config files in your experiment folder. 