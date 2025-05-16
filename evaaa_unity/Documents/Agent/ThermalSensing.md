# ThermalSensing.cs

## Overview
The `ThermalSensing` component enables agents in EVAAA to sense and respond to local environmental temperature. It provides agents with thermal perception by reading from a spatial temperature grid, supporting adaptive behavior in dynamic environments. Nearly all aspects of thermal sensing are controlled through the agent's config file and the environment's thermal grid config—making it easy for both beginners and advanced users to customize behavior without coding.

![sup_fig3.png](/image/sup_fig3.png)

## How to Use
- **For beginners:**
  - You can control whether the agent senses temperature and how it interprets thermal data by editing the relevant fields in `agentConfig.json` (e.g., `useThermalObs`, `relativeThermalObs`, `thermoLevelRange`, `startThermoLevel`).
  - The environment's temperature grid is set up in `thermoGridConfig.json` (e.g., grid size, default temperature, hot spots).
  - No coding is required—just open the files in a text editor, change values, and press Play in Unity.
- **For advanced users:**
  - You can extend or modify the thermal sensing logic by editing `ThermalSensing.cs` in `Assets/Scripts/Agent/`.

## Configuration Reference
Below is a list of all relevant config fields for thermal sensing, with explanations and examples:

**From `agentConfig.json`:**
| Field | Type/Format | Example | Description |
|-------|-------------|---------|-------------|
| `useThermalObs` | bool | `true` | If true, agent receives thermal sensor input. |
| `relativeThermalObs` | bool | `false` | If true, thermal input is relative to agent's body temperature. |
| `thermoLevelRange` | object `{min, max}` | `{ "min": -15.0, "max": 15.0 }` | Allowed range for body temperature. |
| `startThermoLevel` | float | `0.0` | Initial body temperature at episode start. |

**From `thermoGridConfig.json`:**
| Field | Type/Format | Example | Description |
|-------|-------------|---------|-------------|
| `numberOfGridCubeX` | int | `100` | Number of grid cubes along the X axis (resolution of the thermal grid). |
| `numberOfGridCubeZ` | int | `100` | Number of grid cubes along the Z axis. |
| `fieldDefaultTemp` | float | `-20.0` | Default temperature of the field. |
| `hotSpotTemp` | float | `20.0` | Temperature of hot spots in the field. |
| `hotSpotCount` | int | `20` | Number of hot spots. |
| `hotSpotSize` | int | `20` | Size of each hot spot. |
| `smoothingSigma` | float | `3.0` | Smoothing parameter for temperature gradients. |
| `useObjectHotSpot` | bool | `true` | If true, objects can act as hot spots. |
| `useRandomHotSpot` | bool | `false` | If true, hot spots are placed randomly. |
| `gridCubeHeight` | int | `2` | Height of each grid cube. |

## Example agentConfig.json (relevant fields)
```json
{
  "useThermalObs": true,
  "relativeThermalObs": false,
  "thermoLevelRange": { "min": -15.0, "max": 15.0 },
  "startThermoLevel": 0.0
}
```

## Example thermoGridConfig.json (relevant fields)
```json
{
  "numberOfGridCubeX": 100,
  "numberOfGridCubeZ": 100,
  "fieldDefaultTemp": -20.0,
  "hotSpotTemp": 20.0,
  "hotSpotCount": 20,
  "hotSpotSize": 20,
  "smoothingSigma": 3.0,
  "useObjectHotSpot": true,
  "useRandomHotSpot": false,
  "gridCubeHeight": 2
}
```

## Main Script Methods & How Config Maps to Behavior
- The agent loads all config fields at startup and uses them to set thermal sensing parameters.
- **Thermal Sensing:** The script reads temperature from the thermal grid at the agent's position and provides this as an observation.
- **Observations:** If `useThermalObs` is true, thermal data is included in the agent's observation vector for learning.
- **Internal State:** `thermoLevelRange` and `startThermoLevel` control the agent's body temperature limits and initial value.
- **Environment:** The thermal grid is set up according to `thermoGridConfig.json` and provides the spatial temperature field.

---

For further details, see the code comments in `Assets/Scripts/Agent/ThermalSensing.cs` or explore the config files in your experiment folder. 