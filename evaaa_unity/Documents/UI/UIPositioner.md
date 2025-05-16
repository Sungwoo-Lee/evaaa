# UIPositioner.cs

## Purpose
The `UIPositioner` class manages the positioning and scaling of UI elements (such as happy and dead face icons) relative to a radial meter in EVAAA. It ensures that overlay icons are correctly placed and sized for clear, consistent visualization.

## Key Features
- Dynamically positions overlay icons (e.g., happy face, dead face) at specific locations around a radial meter.
- Scales icons proportionally to the size of the radial meter for consistent appearance.
- Supports configurable offset and scaling factors.
- Updates positions and sizes in real time during simulation.

## Configuration Options
- `radialMeterRect`: RectTransform of the radial meter UI element.
- `happyFaceRect`: RectTransform of the happy face icon.
- `deadFaceRect`: RectTransform of the dead face icon.
- `faceOffset`: Additional offset for icon positioning.
- `faceScaleFactor`: Percentage of radial meter size used for icon scaling.

## Main Methods
- `Update()`: Calls the positioning and scaling logic each frame.
- `PositionAndScaleFaces()`: Calculates and applies positions and sizes for overlay icons.

## Integration Notes
- Attach the `UIPositioner` script to a UI GameObject in the scene.
- Assign the radial meter and icon RectTransforms in the Inspector.
- Works in conjunction with other UI scripts for enhanced visualization.

## Example Usage
- Attach `UIPositioner` to a UI GameObject in the scene.
- Assign the radial meter and icon RectTransforms in the Inspector.
- The icons will automatically be positioned and scaled relative to the radial meter during simulation.

---

For more details on UI setup and advanced usage, see the code comments in `Assets/Scripts/UI/UIPositioner.cs`. 