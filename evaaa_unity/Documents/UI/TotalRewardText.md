# TotalRewardText.cs

## Purpose
The `TotalRewardText` (or `TotalRewardDisplay`) class provides a real-time UI display of the agent's current and average reward, as well as the episode number, in EVAAA. It helps users and researchers monitor agent performance during simulation runs.

## Key Features
- Displays the current episode number and agent reward in a TextMeshProUGUI element.
- Optionally shows average reward over a configurable window of steps.
- Dynamically updates the UI each frame based on agent and data recorder state.
- Can be extended to color-code the reward text based on value.

## Configuration Options
- `agentState`: Reference to the `InteroceptiveAgent` whose reward is displayed.
- `totalRewardText`: Reference to the TextMeshProUGUI component for displaying reward info.

## Main Methods
- `Start()`: Initializes references, including the data recorder.
- `Update()`: Updates the reward and episode display each frame.

## Integration Notes
- Attach the `TotalRewardText` script to a UI GameObject in the scene.
- Assign the agent and TextMeshProUGUI references in the Inspector.
- Works in conjunction with the agent's reward system and the data recorder for episode tracking.

## Example Usage
- Attach `TotalRewardText` to a UI GameObject in the scene.
- Assign the agent and text references in the Inspector.
- The UI will automatically update during simulation to reflect the agent's reward and episode number.

---

For more details on UI setup and advanced usage, see the code comments in `Assets/Scripts/UI/TotalRewardText.cs`. 