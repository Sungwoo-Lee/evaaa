# ConfigurableCameraSensor.cs

## Overview
The `ConfigurableCameraSensor` component provides agents in EVAAA with flexible, attachable camera vision for reinforcement learning. It enables visual observation input (RGB or grayscale) and supports a range of camera configurations, including resolution, compression, and field of view. Nearly all aspects of the camera sensor are controlled through the script's public fields and, optionally, the environment's camera config file—making it easy for both beginners and advanced users to customize agent vision without coding.

## How to Use
- **For beginners:**
  - You can control the agent's camera vision by editing the relevant fields in the Unity Inspector (e.g., width, height, grayscale, compressionType, sensorName) or by modifying `cameraConfig.json` (e.g., camera position and angle).
  - No coding is required—just set the values and press Play in Unity.
- **For advanced users:**
  - You can extend or modify the camera sensor logic by editing `ConfigurableCameraSensor.cs` in `Assets/Scripts/Agent/`.

## Configuration Reference
Below is a list of all relevant config fields for the camera sensor, with explanations and examples:

**From `cameraConfig.json`:**
| Field | Type/Format | Example | Description |
|-------|-------------|---------|-------------|
| `initCameraPosition` | object `{x, y, z}` | `{ "x": 0.0, "y": 10.0, "z": -10.0 }` | Initial position of the camera relative to the agent. |
| `initCameraAngle` | object `{x, y, z}` | `{ "x": 30.0, "y": 0.0, "z": 0.0 }` | Initial rotation of the camera (Euler angles). |

## Example cameraConfig.json (relevant fields)
```json
{
  "initCameraPosition": { "x": 0.0, "y": 10.0, "z": -10.0 },
  "initCameraAngle": { "x": 30.0, "y": 0.0, "z": 0.0 }
}
```

## Main Script Methods & How Config Maps to Behavior
- The agent loads all config fields at startup and uses them to set camera sensor parameters.
- **Initialization:** The script automatically attaches or creates a camera for the agent and sets its parameters.
- **Observation:** The camera captures images at the specified resolution, color mode, and compression, and provides them as observations to the agent.
- **Runtime Updates:** Methods like `UpdateSensorDimensions()` and `UpdateGrayscale()` allow dynamic changes to camera settings during simulation.

---

For further details, see the code comments in `Assets/Scripts/Agent/ConfigurableCameraSensor.cs` or explore the config files in your experiment folder. 