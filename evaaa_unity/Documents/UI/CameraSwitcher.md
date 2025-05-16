# CameraSwitcher.cs

## Purpose
The `CameraSwitcher` class provides a flexible UI and camera management system for toggling between first-person and third-person views in EVAAA. It enables users to switch perspectives and dynamically updates UI panels and overlays based on the current view mode.

## Key Features
- Switches between first-person (agent view) and third-person (overview) camera modes.
- Supports toggling via a configurable keyboard key (default: Y).
- Dynamically resizes and positions the agent's first-person view (RawImage) for full-screen or inset display.
- Activates/deactivates UI panels (radial meters, thermal grid, reward text) based on view mode.
- Integrates with heatmap and agent tracking overlays.
- Supports both AI-controlled and human-controlled modes.

## Configuration Options
- `thirdPersonCamera`: Reference to the third-person Camera component.
- `agentView`: RawImage displaying the agent's first-person view.
- `switchKey`: Keyboard key for toggling views.
- `radialMeterPanel`, `thermoSensorGridPanel`, `totalRewardText`: UI panels to show/hide per view mode.
- `isAIControlled`: Whether the agent is AI-controlled (affects default view).
- `heatMapScript`: Reference to the HeatMap script for overlay management.
- `resourceUIScript`: Reference to the ResourceUI script for UI management.

## Main Methods
- `Awake()`: Validates references and stores original camera settings.
- `Start()`: Caches UI references and initializes the default view.
- `Update()`: Listens for the switch key to toggle views.
- `InitializeFirstPersonView()`: Sets up UI and camera for first-person mode.
- `InitializeThirdPersonView()`: Sets up UI and camera for third-person mode.
- `ToggleView()`: Switches between view modes.

## Integration Notes
- Attach the `CameraSwitcher` script to a UI GameObject in the scene.
- Assign all required camera, UI, and script references in the Inspector.
- Works in conjunction with other UI scripts (ResourceUI, HeatMap, etc.) for coordinated display.

## Example Usage
- Attach `CameraSwitcher` to a UI GameObject in the scene.
- Assign camera, agent view, and UI panel references in the Inspector.
- Press the switch key (default: Y) during simulation to toggle views.

---

For more details on UI setup and advanced usage, see the code comments in `Assets/Scripts/UI/CameraSwitcher.cs`. 