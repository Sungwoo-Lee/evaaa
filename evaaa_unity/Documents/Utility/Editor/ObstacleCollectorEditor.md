# ObstacleCollectorEditor.cs

## Purpose
The `ObstacleCollectorEditor` class provides a custom Unity Editor inspector for the `ObstacleCollector` script in EVAAA. It adds a user-friendly button to the Inspector, allowing users to trigger obstacle collection and export directly from the Unity Editor.

## Key Features
- Customizes the Inspector for GameObjects with the `ObstacleCollector` component.
- Adds a "Collect Obstacles" button to the Inspector UI.
- Allows users to collect and export obstacle configuration data with a single click.
- Integrates seamlessly with Unity's Editor scripting system.

## Main Methods
- `OnInspectorGUI()`: Draws the default inspector and adds the custom button. Calls `CollectObstacles()` when clicked.

## Integration Notes
- Place `ObstacleCollectorEditor.cs` in an `Editor` folder within your project (e.g., `Assets/Scripts/Utility/Editor`).
- The custom inspector will automatically appear for any GameObject with the `ObstacleCollector` component.
- Enhances workflow for environment setup and configuration export.

## Example Usage
- Attach `ObstacleCollector` to a GameObject in the scene.
- In the Unity Editor, select the GameObject and click the "Collect Obstacles" button in the Inspector to export obstacle configuration.

---

For more details on editor scripting and advanced usage, see the code comments in `Assets/Scripts/Utility/Editor/ObstacleCollectorEditor.cs`. 