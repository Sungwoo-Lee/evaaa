# CourtSpawner.cs

## Overview
The `CourtSpawner` is the component that creates the physical arena (court) in EVAAA. It defines the navigable space, boundaries, and visual appearance for all experiments. Nearly all aspects of the court are controlled through a simple JSON config file—making it easy for both beginners and advanced users to customize the environment without coding.

## How to Use
- **For beginners:**
  - You can change the size, position, and appearance of the arena by editing the `courtConfig.json` file in your chosen config folder (e.g., `Config/exp-Ymaze/courtConfig.json`).
  - No coding is required—just open the file in a text editor, change values, and press Play in Unity.
- **For advanced users:**
  - You can extend or modify the court logic by editing `CourtSpawner.cs` in `Assets/Scripts/Environment/`.

## Configuration Reference
Below is a complete list of all config fields in `courtConfig.json`, with types, examples, and clear descriptions:

| Field                | Type/Format         | Example | Description |
|----------------------|--------------------|---------|-------------|
| `floorSize`          | object `{x,y,z}`   | `{ "x": 60, "y": 1, "z": 80 }` | Size of the court floor (width, height, depth). |
| `wallHeight`         | float/int           | `5`     | Height of the boundary walls. |
| `position`           | object `{x,y,z}`   | `{ "x": 0, "y": 0, "z": 0 }` | Center position of the court in world space. |
| `floorMaterialName`  | string              | `"FloorMaterial-gray"` | Name of the floor material (must exist in `Resources/Materials`). |
| `wallMaterialName`   | string              | `"WallMaterial"` | Name of the wall material (must exist in `Resources/Materials`). |
| `createWall`         | bool                | `false` | If true, creates boundary walls; if false, arena is unbounded. |

## Example courtConfig.json
```json
{
    "floorSize": { "x": 60, "y": 1, "z": 80 },
    "wallHeight": 5,
    "position": { "x": 0, "y": 0, "z": 0 },
    "floorMaterialName": "FloorMaterial-gray",
    "wallMaterialName": "WallMaterial",
    "createWall": false
}
```

## Main Script Methods & How Config Maps to Behavior
- The court config is loaded at runtime using the `ConfigLoader` utility, with the file specified by `configFileName` (default: `courtConfig.json`).
- **Initialization:** `InitializeCourt()` reads the config and sets up the court.
- **Floor:** `floorSize` sets the width and depth of the arena; `floorMaterialName` sets the floor's appearance.
- **Walls:** `createWall` toggles boundary walls; `wallHeight` and `wallMaterialName` set their size and appearance.
- **Position:** `position` sets the center of the court in world space.
- The script ensures all materials are loaded from `Resources/Materials` and applies them to the generated objects.

## Practical Tips for Research & Tuning
- **Arena Size:** Larger `floorSize` increases navigable area and task complexity.
- **Boundaries:** Use `createWall: true` for bounded tasks (e.g., for novice agents or predator-prey); set to `false` for open environments.
- **Materials:** Ensure material names match those in your Unity project under `Resources/Materials`.
- **Reproducibility:** Always save and document your config files for each experiment.
- **Debugging:** Use distinct materials for floor and walls to visually distinguish boundaries.

## Further Details
See the code in `Assets/Scripts/Environment/CourtSpawner.cs` for implementation details, or experiment with different configs in the `Config/` folders. All fields are documented above for easy mapping between config, code, and experiment design. 