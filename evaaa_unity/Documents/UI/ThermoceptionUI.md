# ThermoceptionUI.cs

## Purpose
The `ThermoceptionUI` (or `ThermoreceptionUI`) class provides a grid-based UI for visualizing the agent's local thermal perception in EVAAA. It displays thermoreceptor values as colored cells and temperature labels, supporting interpretability and debugging of thermal sensing.

## Key Features
- Displays a grid of thermoreceptor values as color-coded UI cells.
- Dynamically updates cell colors and temperature labels based on agent observations.
- Maps temperature deviations to color gradients (green for set-point, red for hot, blue for cold).
- Supports flexible grid size and label assignment.
- Integrates with Unity UI (Image, TextMeshProUGUI).

## Configuration Options
- `thermoreceptionGrid`: Reference to the parent GameObject containing grid cells.
- `interoceptiveAgent`: Reference to the agent providing thermoreceptor data.
- `gridCells`: List of UI Image components for each cell (auto-assigned if empty).
- `temperatureLabels`: List of TextMeshProUGUI components for each cell (auto-assigned if empty).
- `setPoint`: Set-point temperature for color mapping.
- `maxDeviation`: Maximum deviation for color scaling.

## Main Methods
- `Start()`: Initializes references, grid cells, and labels.
- `Update()`: Updates thermoreceptor data and UI each frame.
- `UpdateThermoreceptionUI()`: Updates cell colors and labels based on current data.
- `GetColorFromTemperature(float temperature)`: Maps temperature to a color gradient.

## Integration Notes
- Attach the `ThermoceptionUI` script to a UI GameObject in the scene.
- Assign the grid parent, agent, and (optionally) cell and label lists in the Inspector.
- Works in conjunction with the agent's thermal sensing system for real-time updates.

## Example Usage
- Attach `ThermoceptionUI` to a UI GameObject in the scene.
- Assign the grid parent, agent, and (optionally) cell and label lists in the Inspector.
- The UI will automatically update during simulation to reflect the agent's thermal perception.

---

For more details on UI setup and advanced usage, see the code comments in `Assets/Scripts/UI/ThermoceptionUI.cs`. 