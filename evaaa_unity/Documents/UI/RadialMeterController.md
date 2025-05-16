# RadialMeterController.cs

## Purpose
The `RadialMeterController` class provides a dynamic, circular UI meter for visualizing an agent's internal state variables (e.g., satiation, hydration, thermal, HP) in EVAAA. It offers smooth transitions and color-coded feedback to enhance interpretability and user experience.

## Key Features
- Displays a radial (circular) meter for any agent state variable.
- Configurable state name, value range, and optimal set-point.
- Smooth transitions for fill amount and color based on state changes.
- Color-codes the meter (optimal, warning, critical) based on deviation from set-point.
- Supports clockwise or counter-clockwise fill direction.
- Integrates with Unity UI (Image, Canvas).

## Configuration Options
- `stateName`: Name of the state variable to display (e.g., "Satiation", "Hydration").
- `minValue`, `maxValue`: Range of the state variable.
- `optimalValue`: Set-point for optimal state.
- `fillClockwise`: Fill direction for the meter.
- `fillImage`: Reference to the UI Image component for the meter.
- `optimalColor`, `warningColor`, `criticalColor`: Colors for different state ranges.
- `warningThreshold`, `criticalThreshold`: Thresholds for color transitions.
- `fillTransitionSpeed`, `colorTransitionSpeed`: Speeds for smooth transitions.
- `agentState`: Reference to the `InteroceptiveAgent` providing state values.

## Main Methods
- `Start()`: Initializes meter settings and references.
- `Update()`: Updates fill amount and color based on the agent's state.
- `CalculateNormalizedFill(float value)`: Normalizes a state value to [0, 1] for the meter.
- `UpdateFillAmount()`: Smoothly updates the fill amount.
- `UpdateFillColor()`: Smoothly updates the fill color based on deviation from optimal.
- `ResetMeter()`: Resets the meter to its initial state.
- `GetCurrentFillAmount()`, `GetCurrentFillColor()`: Accessors for current meter state.

## Integration Notes
- Attach the `RadialMeterController` script to a UI GameObject in the scene.
- Assign the fill image and configure state parameters in the Inspector.
- Works in conjunction with the agent's internal state system for real-time updates.

## Example Usage
- Attach `RadialMeterController` to a UI GameObject in the scene.
- Assign the fill image and configure state parameters in the Inspector.
- Assign the agent reference.
- The meter will automatically update during simulation to reflect the agent's internal state.

---

For more details on UI setup and advanced usage, see the code comments in `Assets/Scripts/UI/RadialMeterController.cs`. 