# MasterInitializer.cs

## Overview
The `MasterInitializer` class orchestrates the initialization of the entire simulation scene in EVAAA. It ensures that all core systems—configuration, spawners, navigation, agent, heatmap, day/night cycle, camera, event system, data recording, and screenshot capture—are set up in the correct order for reproducible experiments. This script is the main entry point for scene setup and experiment reproducibility.

## How to Use
- **For beginners:**
  - You do not need to modify this script for most experiments. Attach it to a GameObject in your Unity scene and assign all required references in the Inspector.
  - Scene setup and initialization will run automatically when you press Play.
- **For advanced users:**
  - You can extend or modify the initialization sequence by editing `MasterInitializer.cs` in `Assets/Scripts/SceneController/`.
  - Add or remove system initialization steps as needed for custom experiments.

## Configuration Reference
- No direct JSON configuration, but relies on `ConfigLoader` for experiment settings (see [ConfigLoader](./ConfigLoader.md)).
- All major system components must be assigned via the Unity Inspector:
  - `ConfigLoader`, `SpawnerManager`, `NavMeshSurface`, `ThermoGridSpawner`, `InteroceptiveAgent`, `HeatMap`, `DayAndNight`, `AgentFollowCamera`, etc.

## Main Script Methods & How Initialization Works
- `Start()`: Entry point; calls `InitializeScene()`.
- `InitializeScene()`: Main initialization sequence for all systems.
- `InitializeSystems()`: Initializes data recording, event system, and screenshot capture.
- `ResetSceneInOrder()`: (Coroutine) Resets the scene in a controlled order for reproducibility.

## Integration Notes
- Designed to be attached to a central GameObject in the Unity scene.
- All referenced components must be assigned in the Unity Inspector.
- Works in conjunction with all major systems for a fully integrated experiment setup.
- Initialization order is critical for reproducibility and correct system behavior.

## Practical Tips
- Always check that all required references are assigned in the Inspector before running an experiment.
- Use the reset functionality to ensure clean experiment restarts.
- For custom setups, extend the initialization sequence as needed.

## Further Details
See the code in `Assets/Scripts/SceneController/MasterInitializer.cs` for implementation details, or explore the config files in the `Config/` folders for practical examples.

## Key Features
- Pauses and resumes the ML-Agents Academy during setup.
- Loads and applies experiment configuration via `ConfigLoader`.
- Initializes all major systems: spawners, NavMesh, thermal grid, agent, heatmap, day/night cycle, camera, event system, data recording, and screenshot capture.
- Dynamically bakes the NavMesh for agent and predator navigation.
- Ensures all dependencies are ready before starting the simulation.
- Provides runtime initialization and reset logic for robust experimentation.

## Main Methods
- `Start()`: Entry point; calls `InitializeScene()`.
- `InitializeScene()`: Main initialization sequence for all systems.
- `InitializeSystems()`: Initializes data recording, event system, and screenshot capture.
- `ResetSceneInOrder()`: (Coroutine) Resets the scene in a controlled order (see code for details).

## Example Usage
- Attach the `MasterInitializer` script to a GameObject in the Unity scene.
- Assign all required component references in the Inspector.
- The script will automatically initialize the scene and all systems when the simulation starts.

---

For more details on advanced usage and initialization order, see the code comments in `Assets/Scripts/SceneController/MasterInitializer.cs`. 