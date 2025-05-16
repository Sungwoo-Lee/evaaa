# ObjectRaycast.cs

## Overview
The `ObjectRaycast` component enables agents in EVAAA to sense obstacles and collisions using a radial raycasting system. This allows agents to perceive their immediate surroundings, detect obstacles, and receive collision-based observations for learning and navigation. Nearly all aspects of raycasting and collision sensing are controlled through the agent's config file—making it easy for both beginners and advanced users to customize behavior without coding.

## How to Use
- **For beginners:**
  - You can control how the agent senses obstacles and collisions by editing the relevant fields in `agentConfig.json` (e.g., `raysPerDirection`, `maxDistance`, `damageConstant`, `useCollisionObs`).
  - No coding is required—just open the file in a text editor, change values, and press Play in Unity.
- **For advanced users:**
  - You can extend or modify the raycasting logic by editing `ObjectRaycast.cs` in `Assets/Scripts/Agent/`.

## Configuration Reference
Below is a list of all relevant config fields for object raycasting and collision, with explanations and examples (from `agentConfig.json`):

| Field | Type/Format | Example | Description |
|-------|-------------|---------|-------------|
| `raysPerDirection` | int | `100` | Number of rays cast in a circle around the agent (higher = finer sensing). |
| `maxDistance` | float | `1.5` | Maximum distance each ray can detect obstacles. |
| `radialRange` | float | `360.0` | Field of view for raycasting (degrees, usually 360 for full circle). |
| `damageConstant` | float | `0.0002` | Scales the amount of damage applied to the agent on collision. |
| `useCollisionObs` | bool | `true` | If true, collision/raycast data is included in the agent's observations. |

## Example agentConfig.json (relevant fields)
```json
{
  "raysPerDirection": 100,
  "maxDistance": 1.5,
  "radialRange": 360.0,
  "damageConstant": 0.0002,
  "useCollisionObs": true
}
```

## Main Script Methods & How Config Maps to Behavior
- The agent loads all config fields at startup and uses them to set raycasting and collision parameters.
- **Raycasting:** `DetectObstacle()` casts rays in a circle, filling the `collisionObservation` array with obstacle data.
- **Collision Handling:** `OnCollisionStay()` and `OnCollisionExit()` apply damage to the agent and update collision state based on config values.
- **Observations:** If `useCollisionObs` is true, raycast/collision data is included in the agent's observation vector for learning.

---

For further details, see the code comments in `Assets/Scripts/Agent/ObjectRaycast.cs` or explore the config files in your experiment folder. 