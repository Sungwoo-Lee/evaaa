# SceneControllers

## Overview
The Scene Controller scripts in EVAAA are responsible for initializing, configuring, and managing the simulation environment. They coordinate the setup of agents, environment components, and supporting systems to ensure reproducible and flexible experiments.

## How to Use
- **For beginners:**
  - You do not need to modify these scripts for most experiments. Scene setup is handled automatically when you select a config folder and press Play in Unity.
  - If you want to customize initialization or add new systems, see the main components below.
- **For advanced users:**
  - You can extend or modify scene logic by editing the scripts below in `Assets/Scripts/SceneController/`.

## Main Components
| Component | Description |
|-----------|-------------|
| [MasterInitializer](./MasterInitializer.md) | Orchestrates the initialization of the entire simulation scene, including configuration, spawners, NavMesh, agent, heatmap, day/night cycle, camera, event system, data recording, and screenshot capture. |
| [ConfigLoader](./ConfigLoader.md) | Loads and manages experiment configuration from JSON files, enabling flexible switching between environments and tasks. |
| [CaptureScreenShot](./CaptureScreenShot.md) | Handles automated screenshot capture and recording during simulation runs, based on configuration settings. |

## Integration Notes
- All components are designed to work together for reproducible, modular experiments.
- Assign all required references in the Unity Inspector for each script.
- See each component's documentation for configuration and usage details.

## Further Details
See the code in `Assets/Scripts/SceneController/` for implementation details, or explore the config files in the `Config/` folders for practical examples. 