# DayAndNight.cs

## Overview
The `DayAndNight` component implements a configurable day-night cycle in the EVAAA environment, driving cyclical changes in lighting, temperature, fog, and camera properties. It enables research on agent adaptation to temporal dynamics, non-stationarity, and environmental periodicity. Nearly all aspects of the cycle are controlled through a simple JSON config file—making it easy for both beginners and advanced users to customize experiments without coding.

## How to Use
- **For beginners:**
  - You can change the speed, temperature, fog, and visual properties of the day-night cycle by editing the `daynightConfig.json` file in your chosen config folder (e.g., `Config/exp-Ymaze/daynightConfig.json`).
  - No coding is required—just open the file in a text editor, change values, and press Play in Unity.
- **For advanced users:**
  - You can extend or modify the temporal logic by editing `DayAndNight.cs` in `Assets/Scripts/Environment/`.

## Configuration Reference
Below is a complete list of all config fields in `daynightConfig.json`, with types, examples, and clear descriptions:

| Field                    | Type/Format | Example   | Description |
|--------------------------|-------------|-----------|-------------|
| `fogChangeSpeed`         | float       | `0.1`     | Speed of fog density changes. |
| `dayFogDensity`          | float       | `0.0`     | Fog density during day. |
| `nightFogDensity`        | float       | `0.1`     | Fog density during night. |
| `dayTemperatureChange`   | float       | `0`       | Temperature offset during day. |
| `nightTemperatureChange` | float       | `0`       | Temperature offset during night. |
| `dayFarClip`             | float/int   | `1000`    | Camera far clipping plane during day. |
| `nightFarClip`           | float/int   | `500`     | Camera far clipping plane during night. |
| `farClipTransitionSpeed` | float/int   | `5`       | Speed of far clip plane adjustments. |
| `randomSunAngle`         | bool        | `false`   | If true, randomizes initial sun position. |
| `rotationIntervalMultiplier` | int     | `30`      | Multiplier for sun rotation intervals. |
| `rotationSpeedSteps`     | int         | `15`      | Steps advanced per update (cycle speed). |
| `temperatureUpdateSteps` | int         | `24`      | Number of steps in temperature cycle (hours per day). |
| `fogExponent`            | float       | `2.0`     | Exponent for non-linear fog transitions. |
| `enableDayNightCycle`    | bool        | `false`   | If false, disables the day-night cycle. |

## Example daynightConfig.json (from exp-Ymaze)
```json
{
    "fogChangeSpeed": 0.1,
    "dayFogDensity": 0.0,
    "nightFogDensity": 0.1,
    "dayTemperatureChange": 0,
    "nightTemperatureChange": 0,
    "dayFarClip": 1000,
    "nightFarClip": 500,
    "farClipTransitionSpeed": 5,
    "randomSunAngle": false,
    "rotationIntervalMultiplier": 30,
    "rotationSpeedSteps": 15,
    "temperatureUpdateSteps": 24,
    "fogExponent": 2.0,
    "enableDayNightCycle": false
}
```

## Main Script Methods & How Config Maps to Behavior
- The day-night config is loaded at runtime using the `ConfigLoader` utility, with the file specified by `configFileName` (default: `daynightConfig.json`).
- **Initialization:** `InitializeDayAndNight()` reads the config and sets up the cycle.
- **Cycle Progression:** The sun rotates, and environmental parameters (fog, temperature, far clip) are updated each step according to the config.
- **Phase Logic:** The script divides the day into five phases (Day, Sunset, Night, DeepNight, Dawn) and applies different settings for each.
- **Integration:** The system can modulate the ThermoGridSpawner and camera properties for full-environment adaptation.

## Practical Tips for Research & Tuning
- **Cycle Speed:** Lower `rotationSpeedSteps` for slower cycles (easier for agents); higher for faster cycles (harder).
- **Thermal Challenge:** Increase `dayTemperatureChange`/`nightTemperatureChange` for greater homeostatic difficulty.
- **Visual Challenge:** Increase `nightFogDensity` and decrease `nightFarClip` for harder vision tasks at night.
- **Reproducibility:** Always save and document your config files for each experiment.
- **Debugging:** Set `enableDayNightCycle` to `false` for static conditions during debugging.

## Further Details
See the code in `Assets/Scripts/Environment/DayAndNight.cs` for implementation details, or experiment with different configs in the `Config/` folders. All fields are documented above for easy mapping between config, code, and experiment design. 