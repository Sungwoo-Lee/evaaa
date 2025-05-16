# Utility System Documentation

## Overview
The Utility System in EVAAA provides foundational data structures, helper functions, and editor tools that support simulation, configuration, and environment setup. These scripts are essential for experiment reproducibility, robust configuration, and efficient development workflows.

---

## Main Components
- **DataRecorder.cs**: Records and exports detailed experiment data at both step and episode levels.
- **Utility.cs**: Provides reusable data structures and helper functions for agent and environment configuration.
- **ObstacleCollector.cs**: Automates the collection and export of obstacle configurations from the Unity scene.
- **Editor/ObstacleCollectorEditor.cs**: Unity Editor extension for obstacle collection.

---

## Usage (Beginner & Advanced)
- **Beginner**: Use the provided data structures (e.g., `ThreeDVector`, `EVRange`, `Coefficient`) in your config files for agents, obstacles, and environments. Attach `DataRecorder` or `ObstacleCollector` to GameObjects in your scene and configure via the Inspector or config files.
- **Advanced**: Integrate utility classes for procedural generation, custom data collection, or advanced spatial reasoning. Use `OverlapUtility.IsOverlapping()` for collision-free placement in custom spawners.

---

## Config Reference Table
| Field/Class         | Type         | Description                                                      | Example/Config File                  |
|---------------------|--------------|------------------------------------------------------------------|--------------------------------------|
| ThreeDVector        | Object       | 3D vector for positions/rotations/scales                         | `initAgentPosition`, `initAgentAngle`|
| ColorVector         | Object       | RGBA color for config                                            | (not shown in default configs)       |
| EVRange             | Object       | Range (min, max) for essential variables                         | `foodLevelRange`, `waterLevelRange`  |
| Coefficient         | Object       | Change coefficients for parameterized updates                    | `foodCoefficient`, `waterCoefficient`|
| PositionRange       | Object       | Range for spatial configuration                                  | `position` in obstacleConfig.json    |
| RotationRange       | Object       | Range for rotation configuration                                 | `rotationRange` in obstacleConfig.json|
| ScaleRange          | Object       | Range for scale configuration                                    | `scaleRange` in obstacleConfig.json  |

---

## Real Example Configs
### From `agentConfig.json`
```json
"initAgentPosition": { "x": 0.0, "y": 1.0, "z": 0.0 },
"foodLevelRange": { "min": -15.0, "max": 15.0 },
"foodCoefficient": {
    "change_0": -0.045,
    "change_1": 0.0,
    "change_2": 0.0,
    "change_3": 0.0,
    "change_4": 0.0,
    "change_5": 0.0
}
```
### From `obstacleConfig.json`
```json
{
    "prefabName": "Tree",
    "position": { "xMin": -45, "xMax": 45, "yMin": 0, "yMax": 0, "zMin": -45, "zMax": 45 },
    "rotationRange": { "x": 0, "y": 0, "z": 0 },
    "scaleRange": { "xMin": 0.8, "xMax": 1.2, "yMin": 0.8, "yMax": 1.2, "zMin": 0.8, "zMax": 1.2 }
}
```

---

## Mapping Config Fields to Script Behavior
- **ThreeDVector**: Used for positions/rotations in agent and environment configs; converted to Unity `Vector3` at runtime.
- **EVRange**: Used for resource/health/thermo ranges; sets min/max values in agent scripts.
- **Coefficient**: Used for resource/health/thermo change rates; mapped to agent update logic.
- **PositionRange/RotationRange/ScaleRange**: Used in obstacle and resource configs; determines placement, orientation, and size during procedural generation.
- **OverlapUtility.IsOverlapping()**: Used by spawners and procedural scripts to ensure no object overlap during placement.

---

## Main Script Methods
- **Utility.cs**: Serializable classes for config, static overlap check utility.
- **DataRecorder.cs**: See DataRecorder.md for full details.
- **ObstacleCollector.cs**: See ObstacleCollector.md for full details.

---

## Practical Tips
- Use the provided data structures in all config files for consistency and Inspector integration.
- Use `OverlapUtility.IsOverlapping()` in custom placement logic to avoid collisions.
- Review code comments in `Assets/Scripts/Utility/Utility.cs` for advanced usage.

---

## Further Details
- Utility scripts are foundational for all config-driven and procedural features in EVAAA.
- Designed for extensibility and easy integration with new modules. 