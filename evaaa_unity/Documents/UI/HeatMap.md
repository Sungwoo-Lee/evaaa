# HeatMap.cs

## Purpose
The `HeatMap` class provides a real-time, grid-based heatmap visualization of environmental temperature in EVAAA. It displays spatial temperature gradients using color mapping, supporting interpretability and analysis of thermal dynamics and agent-environment interactions.

## Key Features
- Visualizes the environment's thermal grid as a color-coded heatmap.
- Dynamically updates the heatmap texture based on temperature data from `ThermoGridSpawner`.
- Supports toggling heatmap and agent track overlays based on agent configuration.
- Integrates with Unity UI (Image, Sprite, Gradient).
- Provides methods for initialization, per-episode updates, and day/night adjustments.

## Configuration Options
- `heatMap`: Reference to the UI Image component displaying the heatmap.
- `agentTrack`: Reference to the GameObject tracking the agent's position.
- `agent`: Reference to the `InteroceptiveAgent` for configuration.
- `gradient`: Gradient used for color mapping of temperatures.

## Main Methods
- `InitializeHeatMap()`: Sets up the heatmap texture and overlays based on the thermal grid.
- `EpisodeHeatMap()`: Updates the heatmap for the current episode.
- `ModifyPixels()`: Updates the heatmap texture pixels based on temperature data.
- `SetDayNightTemperature(bool isNight)`: Updates the heatmap for day/night cycles.
- `IsThermalGridReady()`: Checks if the thermal grid is ready for visualization.

## Integration Notes
- Attach the `HeatMap` script to a UI GameObject in the scene.
- Assign the heatmap image, agent track, and gradient in the Inspector.
- Works in conjunction with `ThermoGridSpawner` and the agent's configuration for real-time updates.

## Example Usage
- Attach `HeatMap` to a UI GameObject in the scene.
- Assign the heatmap image, agent track, and gradient in the Inspector.
- Call `InitializeHeatMap()` at runtime to set up the visualization.
- Call `EpisodeHeatMap()` to update the heatmap each episode.

---

For more details on UI setup and advanced usage, see the code comments in `Assets/Scripts/UI/HeatMap.cs`. 