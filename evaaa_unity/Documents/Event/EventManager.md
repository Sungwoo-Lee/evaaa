# EventManager.cs

## Overview
The `EventManager` class manages the configuration, instantiation, and handling of in-game events and triggers in EVAAA. It enables dynamic, event-driven scenarios by generating trigger zones and registering event handlers based on external JSON configuration files. Nearly all aspects of event logic are controlled through a simple JSON config file—making it easy for both beginners and advanced users to customize experiments without coding.

## How to Use
- **For beginners:**
  - You can add or modify in-game events by editing the `eventConfig.json` file in your chosen config folder (e.g., `Config/exp-goal-manipulation-FoodToWater/eventConfig.json`).
  - No coding is required—just open the file in a text editor, change values, and press Play in Unity.
- **For advanced users:**
  - You can extend or modify event logic by editing `EventManager.cs` in `Assets/Scripts/Event/`.

## Configuration Reference
Below is a complete list of all config fields in `eventConfig.json`, with types, examples, and clear descriptions. Events are defined in groups:

| Field         | Type/Format | Example | Description |
|--------------|-------------|---------|-------------|
| `name`       | string      | `"ResourceTrigger"` | Name of the event trigger GameObject. |
| `triggerTag` | string      | `"resource"` | Tag for the event trigger (used to register/trigger events). |
| `targetTag`  | string      | `"player"` | Tag for objects that can activate the trigger. |
| `maxCount`   | int         | `1`     | Maximum number of times the event can be triggered (-1 for unlimited). |
| `position`   | object `{x,y,z}` | `{ "x": -13.0, "y": -0.5, "z": 0.0 }` | Position of the trigger in the environment. |
| `rotation`   | object `{x,y,z}` | `{ "x": 0.0, "y": 0.0, "z": 0.0 }` | Rotation of the trigger. |
| `scale`      | object `{x,y,z}` | `{ "x": 1.0, "y": 7.0, "z": 30.0 }` | Scale of the trigger zone. |
| `foodValue`  | float       | `7.0`   | Food value to set on trigger (for resource events). |
| `waterValue` | float       | `-7.0`  | Water value to set on trigger (for resource events). |
| `message`    | string      | `""`   | Message to display/log on trigger (for message events). |

## Example eventConfig.json (from exp-goal-manipulation-FoodToWater)
```json
{
    "groups": [
        {
            "name": "ResourceTrigger",
            "triggerTag": "resource",
            "targetTag": "player",
            "maxCount": 1,
            "position": { "x": -13.0, "y": -0.5, "z": 0.0 },
            "rotation": { "x": 0.0, "y": 0.0, "z": 0.0 },
            "scale": { "x": 1.0, "y": 7.0, "z": 30.0 },
            "foodValue": 7.0,
            "waterValue": -7.0,
            "message": ""
        }
    ]
}
```

## Main Script Methods & How Config Maps to Behavior
- The event config is loaded at runtime using the `ConfigLoader` utility, with the file specified by `configFileName` (default: `eventConfig.json`).
- **Initialization:** `InitializeEventManager()` reads the config and sets up all event triggers and handlers.
- **Trigger Generation:** Each group spawns a trigger zone in the environment, with position, rotation, and scale from the config.
- **Event Registration:** Each group registers an event handler for its `triggerTag` (resource or message events).
- **Event Handling:**
  - Resource events update agent food/water levels according to `foodValue` and `waterValue`.
  - Message events log/display a message and can record the event.
- **Prefab Loading:** Triggers are created as GameObjects with colliders and the `TriggerZone` component.
- **Resetting:** The system supports runtime reset and reinitialization of all events and triggers (e.g., via keyboard input or script call).

## Practical Tips for Research & Tuning
- **Event Design:** Use multiple event groups for complex scenarios (e.g., resource zones, message popups, checkpoints).
- **Max Count:** Set `maxCount` to limit how many times an event can be triggered (useful for one-time rewards or messages).
- **Debugging:** Use unique `name` and `triggerTag` values for each event group to track triggers in logs.
- **Reproducibility:** Always save and document your config files for each experiment.
- **Integration:** The EventManager works with the `GameEventSystem` for event registration and triggering, and integrates with agent scripts and data recording for event logging.

## Further Details
See the code in `Assets/Scripts/Event/EventManager.cs` for implementation details, or experiment with different configs in the `Config/` folders. All fields are documented above for easy mapping between config, code, and experiment design.

## Key Features
- Loads event configuration from JSON files for flexible scenario design.
- Dynamically generates trigger zones in the environment based on configuration.
- Registers event handlers for different event types (e.g., resource change, message feedback).
- Supports both resource-based and message-based events.
- Integrates with agent state, data recording, and environment systems.
- Provides runtime reset and reinitialization of events.

## Configuration Options
- `configFileName`: Name of the event configuration JSON file (default: `eventConfig.json`).
- Event groups: Each group specifies trigger tag, target tag, resource values, message, position, rotation, scale, and max count.

## Main Methods
- `InitializeEventManager(ConfigLoader loader)`: Loads configuration and sets up triggers and handlers.
- `GenerateTriggers()`: Instantiates trigger zones in the environment.
- `RegisterEventHandlers()`: Registers event handlers for each event group.
- `HandleMessageEvent(GameObject obj)`: Handles message-based events.
- `HandleResourceEvent(GameObject obj)`: Handles resource-based events (e.g., food, water changes).
- `ResetEventManager()`: Resets and reinitializes all events and triggers.
- `ClearTriggers()`: Removes all dynamically generated triggers.
- `FixedUpdate()`: Allows runtime reset of event counts via keyboard input.

## Integration Notes
- Designed to work with the `GameEventSystem` for event registration and triggering.
- Trigger zones are implemented using the `GameEventSystem.TriggerZone` component.
- Integrates with agent scripts (e.g., `InteroceptiveAgent`) and data recording for event logging.
- Configuration is managed via JSON files in the `Config/` directory.

## Example Usage
- Attach the `EventManager` script to a GameObject in the Unity scene.
- Provide a valid event configuration JSON file in the `Config/` directory.
- Call `InitializeEventManager` with a reference to the `ConfigLoader` at runtime.
- The system will automatically generate triggers and register event handlers.

---

For more details on configuration fields and advanced usage, see the code comments in `Assets/Scripts/Event/EventManager.cs`. 