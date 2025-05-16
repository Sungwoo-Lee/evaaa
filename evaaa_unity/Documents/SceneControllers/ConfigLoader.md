# ConfigLoader.cs

## Overview
The `ConfigLoader` class loads and manages experiment configuration files in EVAAA. It enables flexible switching between training levels, experimental testbeds, and custom scenarios by reading structured JSON files from the `Config/` directory. This script is the central hub for all experiment settings.

## How to Use
- **For beginners:**
  - You do not need to modify this script for most experiments. Attach it to a GameObject in your Unity scene and assign the main configuration file name in the Inspector (if different from default).
  - Place all configuration files in the `Config/` directory at the project root.
  - The script will automatically load the correct settings when you press Play.
- **For advanced users:**
  - You can extend or modify config loading logic by editing `ConfigLoader.cs` in `Assets/Scripts/SceneController/`.
  - Use the provided methods to load custom config files or add new config types.

## Configuration Reference
Below is a list of main config fields in `mainConfig.json`, with types, examples, and clear descriptions:

| Field                | Type/Format | Example | Description |
|----------------------|-------------|---------|-------------|
| `isAIControlled`     | bool        | `true`  | Whether the agent is controlled by AI or human. |
| `configFolderName`   | string      | `"exp-goal-manipulation-WaterToFood"` | Name of the active experiment config folder. |
| `recordingScreen`    | object      | `{ "recordEnable": "false", "recordingFolderName": "SampleRecording" }` | Screenshot recording options. |
| `experimentData`     | object      | `{ "recordEnable": "false", "baseFolderName": "ExperimentData", "fileNamePrefix": "data" }` | Data recording options. |

## Example mainConfig.json
```json
{
    "isAIControlled": true,
    "configFolderName": "exp-goal-manipulation-WaterToFood",
    "recordingScreen": {
        "recordEnable": "false",
        "recordingFolderName": "SampleRecording"
    },
    "experimentData": {
        "recordEnable": "false",
        "baseFolderName": "ExperimentData",
        "fileNamePrefix": "data"
    }
}
```

## Main Script Methods & How Config Maps to Behavior
- `InitializeConfigLoader()`: Loads the main configuration and sets the active experiment folder.
- `LoadMainConfig()`: Reads and parses the main configuration JSON file.
- `SetConfigFolder(string folderName)`: Sets the path to the experiment folder.
- `LoadConfig<T>(string configFileName)`: Loads and deserializes a configuration file of type `T` from the active folder.
- `GetFullPath(string configFileName)`: Returns the full path to a configuration file in the active folder.

## Integration Notes
- Designed to be attached to a GameObject in the Unity scene.
- Used by `MasterInitializer` and other systems to load and manage experiment settings.
- All configuration files should be placed in the `Config/` directory at the project root.
- Supports both Windows and macOS builds.

## Practical Tips
- Always check that your config folder and file names match those in your experiment setup.
- Use the Inspector to quickly switch between different experiment folders.
- For custom experiments, add new config files to your folder and load them using `LoadConfig<T>()`.

## Further Details
See the code in `Assets/Scripts/SceneController/ConfigLoader.cs` for implementation details, or explore the config files in the `Config/` folders for practical examples. 