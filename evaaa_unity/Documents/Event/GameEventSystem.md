# GameEventSystem.cs

## Overview
The `GameEventSystem` class provides a static, centralized event management system for EVAAA. It enables registration, triggering, and management of in-game events, supporting flexible event-driven interactions between agents, triggers, and the environment. It is used by the `EventManager` and can be extended or used directly for custom event-driven logic.

## How to Use
- **For beginners:**
  - You do not need to modify this script directly for most experiments. Events are configured via JSON and managed by the `EventManager`.
  - If you want to add custom event logic, you can register new event handlers in your own scripts using the API below.
- **For advanced users:**
  - You can extend or modify the event system by editing `GameEventSystem.cs` in `Assets/Scripts/Event/`.
  - Use the static API to register, trigger, and manage events programmatically.

## API Reference & Main Methods
| Method | Example Usage | Description |
|--------|--------------|-------------|
| `Register(string triggerTag, Action<GameObject> action, int maxCount = -1)` | `GameEventSystem.Register("resource", HandleResourceEvent, 5);` | Registers an event handler for a tag, with optional max trigger count. |
| `Trigger(string triggerTag, GameObject invoker)` | `GameEventSystem.Trigger("resource", agentGameObject);` | Triggers all handlers for a tag. Usually called by a `TriggerZone`. |
| `ResetCount(string triggerTag)` | `GameEventSystem.ResetCount("resource");` | Resets the trigger count for a tag. |
| `GetCount(string triggerTag)` | `int count = GameEventSystem.GetCount("resource");` | Returns the current trigger count for a tag. |
| `ClearAllEventHandlers()` | `GameEventSystem.ClearAllEventHandlers();` | Removes all registered event handlers and resets counts. |

### TriggerZone Component
| Method | Example Usage | Description |
|--------|--------------|-------------|
| `SetTriggerTag(string tag)` | `triggerZone.SetTriggerTag("resource");` | Sets the trigger tag for the zone. |
| `SetTargetTag(string tag)` | `triggerZone.SetTargetTag("player");` | Sets the target tag for objects that can activate the zone. |
| `OnTriggerEnter(Collider other)` | *(automatic)* | Triggers the event when a target-tagged object enters the zone. |

## Example: Registering and Triggering Events
```csharp
// Register an event handler for the "resource" tag, max 5 triggers
GameEventSystem.Register("resource", HandleResourceEvent, 5);

// Trigger the event (usually called by TriggerZone)
GameEventSystem.Trigger("resource", agentGameObject);

// Reset the trigger count for reproducibility
GameEventSystem.ResetCount("resource");

// Clear all event handlers (e.g., at experiment reset)
GameEventSystem.ClearAllEventHandlers();
```

## Practical Tips
- **Integration:** Use with `EventManager` for data-driven event scenarios, or register your own handlers for custom logic.
- **Max Count:** Use the `maxCount` parameter to limit how many times an event can be triggered (e.g., for one-time rewards).
- **Debugging:** Use unique trigger tags for each event type to track and debug event flow.
- **Reproducibility:** Always reset event counts and handlers between experiments for clean results.

## Further Details
See the code in `Assets/Scripts/Event/GameEventSystem.cs` for implementation details, or use the API above in your own scripts for advanced event-driven logic. The system is designed for extensibility and integration with agent and environment systems. 