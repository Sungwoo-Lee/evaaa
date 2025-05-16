# AgentTrackBalckDot.cs

## Purpose
The `AgentTrackBalckDot` (or `AgentTrackBlackDot`) class provides a UI overlay for tracking the agent's position on a heatmap or court map in EVAAA. It visually represents the agent's real-time location as a moving dot, aiding in spatial analysis and debugging.

## Key Features
- Tracks the agent's position relative to the environment (court) and maps it to a UI overlay.
- Moves a black dot (UI element) to represent the agent's current location on the heatmap.
- Supports dynamic updates in real time using Unity's `FixedUpdate`.
- Integrates with environment and UI components for accurate mapping.

## Configuration Options
- `agent`: Reference to the agent GameObject being tracked.
- `courtSpawner`: Reference to the `CourtSpawner` for environment dimensions.
- `heatMapRect`: RectTransform of the heatmap UI element.
- `blackDotRect`: RectTransform of the black dot UI element.

## Main Methods
- `Awake()`: Validates all required references.
- `FixedUpdate()`: Updates the black dot's position each physics frame.
- `GetAgentRelativePosition()`: Calculates the agent's normalized position within the court.
- `MoveBlackDot(Vector3 relativePosition)`: Moves the black dot on the UI based on the agent's relative position.

## Integration Notes
- Attach the `AgentTrackBalckDot` script to a UI GameObject in the scene.
- Assign the agent, court spawner, heatmap, and black dot references in the Inspector.
- Works in conjunction with the court/environment and heatmap UI for real-time tracking.

## Example Usage
- Attach `AgentTrackBalckDot` to a UI GameObject in the scene.
- Assign all required references in the Inspector.
- The black dot will automatically track the agent's position on the heatmap during simulation.

---

For more details on UI setup and advanced usage, see the code comments in `Assets/Scripts/UI/AgentTrackBalckDot.cs`. 