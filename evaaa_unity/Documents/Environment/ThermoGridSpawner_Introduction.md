# ThermoGridSpawner.cs

## Overview
The `ThermoGridSpawner` is a core environment component in EVAAA, responsible for creating a spatially-distributed temperature field that agents must navigate to maintain homeostasis. Inspired by biological thermoregulation, it enables research on homeostatic RL, adaptive behavior, and embodied intelligence. Nearly all aspects of the thermal environment are controlled through a simple JSON config file—making it easy for both beginners and advanced users to customize experiments without coding.

## How to Use
- **For beginners:**
  - You can change the temperature landscape by editing the `thermoGridConfig.json` file in your chosen config folder (e.g., `Config/exp-Ymaze/thermoGridConfig.json`).
  - No coding is required—just open the file in a text editor, change values, and press Play in Unity.
- **For advanced users:**
  - You can extend or modify the grid logic by editing `ThermoGridSpawner.cs` in `Assets/Scripts/Environment/`.

## Configuration Reference
Below is a complete list of all config fields in `thermoGridConfig.json`, with types, examples, and clear descriptions:

| Field                | Type/Format | Example | Description |
|----------------------|-------------|---------|-------------|
| `numberOfGridCubeX`  | int         | `100`   | Number of grid cells along X (width). Sets grid resolution and observation detail. |
| `numberOfGridCubeZ`  | int         | `100`   | Number of grid cells along Z (depth). |
| `fieldDefaultTemp`   | float       | `0.0`   | Baseline temperature for all cells. Sets the neutral point for agent homeostasis. |
| `hotSpotTemp`        | float       | `20.0`  | Temperature value for hot spots (thermal extremes). |
| `hotSpotCount`       | int         | `0`     | Number of hot spots to place (if `useRandomHotSpot` is true). |
| `hotSpotSize`        | float       | `20`    | Size (radius) of each hot spot. |
| `smoothingSigma`     | float       | `5.0`   | Controls Gaussian smoothing (gradient sharpness). |
| `useObjectHotSpot`   | bool        | `true`  | If true, links hot spots to objects (e.g., obstacles). |
| `useRandomHotSpot`   | bool        | `true`  | If true, places hot spots randomly. |
| `gridCubeHeight`     | float/int   | `2`     | Visual height of grid cubes (for debugging/visualization). |

## Example thermoGridConfig.json
```json
{
    "numberOfGridCubeX": 100,
    "numberOfGridCubeZ": 100,
    "fieldDefaultTemp": 0.0,
    "hotSpotTemp": 20.0,
    "hotSpotCount": 0,
    "hotSpotSize": 20,
    "smoothingSigma": 5.0,
    "useObjectHotSpot": true,
    "useRandomHotSpot": true,
    "gridCubeHeight": 2
}
```

## Main Script Methods & How Config Maps to Behavior
- The grid config is loaded at runtime using the `ConfigLoader` utility, with the file specified by `configFileName` (default: `thermoGridConfig.json`).
- **Initialization:** `InitializeThermoGridSpawner()` and `InitializeGrid()` read the config and set up the grid.
- **Temperature Assignment:**
  - `fieldDefaultTemp` sets the baseline for all grid cells.
  - `hotSpotTemp`, `hotSpotCount`, `hotSpotSize`, `useRandomHotSpot`, and `useObjectHotSpot` control the placement and properties of hot spots.
  - `smoothingSigma` applies Gaussian smoothing for realistic gradients.
- **Visualization:** `gridCubeHeight` sets the height of grid cubes (can be used for debugging).
- **Dynamic Updates:** The grid can update in response to day/night cycles and agent/environment actions via methods like `SetDayNightTemperature()`.

## Practical Tips for Research & Tuning
- **Resolution:** Higher `numberOfGridCubeX/Z` increases spatial detail but uses more memory/compute.
- **Difficulty:** Increase `hotSpotCount` or decrease `hotSpotSize` for more challenging navigation.
- **Gradient Sharpness:** Lower `smoothingSigma` for sharper gradients, higher for smoother fields.
- **Reproducibility:** Always save and document your config files for each experiment.
- **Debugging:** Set `gridCubeHeight` > 0 and enable rendering to visualize the grid in Unity.

## Further Details
See the code in `Assets/Scripts/Environment/ThermoGridSpawner.cs` for implementation details, or experiment with different configs in the `Config/` folders. All fields are documented above for easy mapping between config, code, and experiment design. 