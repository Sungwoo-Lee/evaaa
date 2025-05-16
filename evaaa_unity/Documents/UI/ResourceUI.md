# ResourceUI.cs

## Purpose
The `ResourceUI` class provides a real-time user interface for visualizing the agent's internal state variables (food, water, thermal, health) in EVAAA. It displays these variables using sliders and color-coded handles, enhancing interpretability and debugging during simulation runs.

## Key Features
- Displays food, water, thermal, and health levels as UI sliders.
- Color-codes each essential variable for quick visual differentiation.
- Dynamically updates UI elements based on the agent's current state.
- Supports enabling/disabling thermal variable display based on agent configuration.
- Integrates with Unity UI (Canvas, Slider, Image, TextMeshPro).

## Configuration Options
- UI references: Assign `Slider`, `Image`, and `GameObject` fields for each variable in the Unity Inspector.
- `agent`: Reference to the `InteroceptiveAgent` whose state is displayed.

## Main Methods
- `Start()`: Initializes UI elements, sets colors, and configures thermal display.
- `FixedUpdate()`: Updates slider values and UI elements based on the agent's resource levels.

## Integration Notes
- Attach the `ResourceUI` script to a UI GameObject (e.g., `ResourceLevelSetting`) under the Canvas.
- Assign all required UI references and the agent reference in the Inspector.
- Works in conjunction with the agent's internal state system for real-time updates.

## Example Usage
- Attach `ResourceUI` to a UI GameObject in the scene.
- Assign sliders, handles, and text objects for each variable in the Inspector.
- Assign the agent reference.
- The UI will automatically update during simulation to reflect the agent's internal state.

---

For more details on UI setup and advanced usage, see the code comments in `Assets/Scripts/UI/ResourceUI.cs`. 