# AgentFollowCamera.cs

## Purpose
The `AgentFollowCamera` class provides a configurable camera system that follows the agent in EVAAA. It supports loading camera position and angle from a configuration file, enabling reproducible and flexible camera setups for experiments and demonstrations.

## Key Features
- Follows the agent GameObject with a configurable offset and rotation.
- Loads camera position and angle from a JSON configuration file (`cameraConfig.json`).
- Supports runtime reloading of camera configuration.
- Integrates with the `ConfigLoader` system for experiment management.
- Updates camera position in `LateUpdate` for smooth following.

## Configuration Options
- `configFileName`: Name of the camera configuration JSON file (default: `cameraConfig.json`).
- `agent`: Reference to the agent GameObject to follow.
- CameraConfig fields: `initCameraPosition`, `initCameraAngle` (set in config file).

## Main Methods
- `InitializeCamera(ConfigLoader loader)`: Loads configuration and sets initial camera rotation.
- `ReloadConfig()`: Reloads camera configuration at runtime.
- `LoadConfig()`: Loads camera configuration from file.
- `LateUpdate()`: Updates camera position each frame to follow the agent.

## Integration Notes
- Attach the `AgentFollowCamera` script to a Camera GameObject in the scene.
- Assign the agent reference and (optionally) the config file name in the Inspector.
- Works in conjunction with the `ConfigLoader` for experiment management.
- Camera position and angle are set via the configuration file for reproducibility.

## Example Usage
- Attach `AgentFollowCamera` to a Camera GameObject in the scene.
- Assign the agent reference and config file name in the Inspector.
- Call `InitializeCamera()` at runtime to set up the camera.
- The camera will automatically follow the agent during simulation.

---

For more details on configuration fields and advanced usage, see the code comments in `Assets/Scripts/UI/AgentFollowCamera.cs`. 