# EventSystem.cs

## Overview
The `Event System` in EVAAA provides a flexible, modular way to manage and trigger in-game events, enabling dynamic interactions between agents and the environment. It supports event-driven scenarios, resource changes, and message-based feedback, all configurable via external JSON files. The system is composed of two main scripts: `EventManager` and `GameEventSystem`.

## How to Use
- **For beginners:**
  - You can add or modify in-game events by editing the `eventConfig.json` file in your chosen config folder (e.g., `Config/exp-goal-manipulation-FoodToWater/eventConfig.json`).
  - No coding is required—just open the file in a text editor, change values, and press Play in Unity.
- **For advanced users:**
  - You can extend or modify event logic by editing `EventManager.cs` and `GameEventSystem.cs` in `Assets/Scripts/Event/`.

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

## Practical Tips for Research & Tuning
- **Event Design:** Use multiple event groups for complex scenarios (e.g., resource zones, message popups, checkpoints).
- **Max Count:** Set `maxCount` to limit how many times an event can be triggered (useful for one-time rewards or messages).
- **Debugging:** Use unique `name` and `triggerTag` values for each event group to track triggers in logs.
- **Reproducibility:** Always save and document your config files for each experiment.

## Further Details
See the code in `Assets/Scripts/Event/EventManager.cs` and `Assets/Scripts/Event/GameEventSystem.cs` for implementation details, or experiment with different configs in the `Config/` folders. All fields are documented above for easy mapping between config, code, and experiment design.

# Event System Overview

The Event System in EVAAA provides a flexible mechanism for managing and triggering in-game events, enabling dynamic interactions between agents and the environment. It supports the creation of event-driven scenarios, resource changes, and message-based feedback, all configurable via external JSON files.

Below are the main components of the Event System. Each component is documented in detail in its own markdown file:

- [EventManager](./EventManager.md): Manages the configuration, instantiation, and handling of in-game events and triggers. Dynamically generates trigger zones and registers event handlers based on configuration files.
- [GameEventSystem](./GameEventSystem.md): Provides a static, centralized event management system for registering, triggering, and managing in-game events. Includes the `TriggerZone` component for collider-based event activation.

For details on each component, see the corresponding markdown file linked above.

## Main Components

### EventManager.cs
- **Purpose:** Manages the configuration, instantiation, and handling of in-game events and triggers.
- **Key Features:**
  - Loads event configurations from JSON files for flexible scenario design.
  - Dynamically generates trigger zones in the environment based on configuration.
  - Registers event handlers for different event types (e.g., resource changes, messages).
  - Handles event logic such as updating agent resources or displaying messages.
  - Supports resetting and reinitializing events during runtime.

### GameEventSystem.cs
- **Purpose:** Provides a static, centralized system for registering, triggering, and managing events and their handlers.
- **Key Features:**
  - Allows registration of event handlers for specific trigger tags, with optional limits on event counts.
  - Triggers all registered actions when a trigger event occurs.
  - Includes a `TriggerZone` component for detecting agent entry and invoking events.
  - Supports resetting event counts and clearing all handlers for experiment reproducibility.

---

The Event System enables modular, data-driven event management, supporting complex experimental designs and adaptive agent-environment interactions in EVAAA. 