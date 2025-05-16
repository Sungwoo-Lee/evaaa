# CaptureScreenShot.cs

## Overview
The `CaptureScreenShot` class handles automated screenshot capture and recording during simulation runs in EVAAA. It enables visual documentation and analysis of agent behavior and environment states, based on experiment configuration settings. Screenshot recording is controlled via the main configuration file.

## How to Use
- **For beginners:**
  - You do not need to modify this script for most experiments. Attach it to a GameObject in your Unity scene.
  - Ensure the main configuration file (`mainConfig.json`) specifies recording options (see below).
  - The script will automatically capture screenshots at specified intervals or events when enabled.
- **For advanced users:**
  - You can extend or modify screenshot logic by editing `CaptureScreenShot.cs` in `Assets/Scripts/SceneController/`.
  - Add custom triggers for screenshot capture as needed.

## Configuration Reference
Below are the relevant fields from `mainConfig.json` for screenshot recording:

| Field                | Type/Format | Example | Description |
|----------------------|-------------|---------|-------------|
| `recordEnable`       | string/bool | `"false"` | Whether screenshot recording is enabled. |
| `recordingFolderName`| string      | `"SampleRecording"` | Name of the output folder for screenshots. |
| `mediaOutputFolder`  | string      | *(auto)* | Full path to the output directory (set automatically). |

## Example mainConfig.json (recording section)
```json
{
    "recordingScreen": {
        "recordEnable": "false",
        "recordingFolderName": "SampleRecording"
    }
}
```

## Main Script Methods & How Config Maps to Behavior
- `Initialize()`: Loads configuration, sets up output directories, and enables recording if configured.
- `CreateRecordDirectory()`: Creates the output directory for screenshots if it does not exist.
- `CaptureImage()`: Captures and saves a screenshot to the output directory.
- `Awake()`: Subscribes to environment reset events for parameter updates.

## Integration Notes
- Designed to be attached to a GameObject in the Unity scene.
- Used by `MasterInitializer` and other systems for automated visual recording.
- Output directory and recording options are set via the main configuration file (`mainConfig.json`).
- Works on both Windows and macOS builds.

## Practical Tips
- Always check that `recordEnable` is set to `true` in your config to enable recording.
- Use descriptive folder names for `recordingFolderName` to organize experiment outputs.
- For custom capture logic, call `CaptureImage()` from your own scripts or events.

## Further Details
See the code in `Assets/Scripts/SceneController/CaptureScreenShot.cs` for implementation details, or explore the config files in the `Config/` folders for practical examples. 