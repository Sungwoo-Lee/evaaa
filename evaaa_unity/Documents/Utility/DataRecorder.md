# DataRecorder Documentation

## Overview
The `DataRecorder` in EVAAA provides comprehensive experiment data recording and export functionality. It tracks agent state, actions, rewards, collisions, resource consumption, and events at both step and episode levels, supporting reproducibility and in-depth analysis of experiments.

---

## Usage (Beginner & Advanced)
- **Beginner**: Attach `DataRecorder` to a GameObject in your Unity scene. Assign the agent reference and configure experiment settings in the Inspector or via config files. Recording is managed automatically during simulation.
- **Advanced**: Use the provided API to record custom events, resource choices, or episode end types. Integrate with custom agent/environment logic for advanced experiment tracking.

---

## Config Reference Table
| Field                | Type    | Description                                              | Example/Config File         |
|----------------------|---------|----------------------------------------------------------|-----------------------------|
| experimentType       | string  | Type of experiment (set in Inspector or config)          | Inspector                   |
| isActive             | bool    | Whether recording is enabled                             | Inspector                   |
| targetAgent          | object  | Reference to the agent being tracked                     | Inspector                   |
| experimentData.recordEnable | bool | Enable/disable data recording                           | mainConfig.json             |
| experimentData.baseFolderName | string | Output directory for data files                        | mainConfig.json             |
| experimentData.fileNamePrefix | string | Prefix for output data files                           | mainConfig.json             |

---

## Real Example Config
From `Config/mainConfig.json`:
```json
"experimentData": {
    "recordEnable": "false",
    "baseFolderName": "ExperimentData",
    "fileNamePrefix": "data"
}
```

---

## Mapping Config Fields to Script Behavior
- **experimentType**: Used to label and organize output files for different experiment types.
- **isActive**: Enables or disables all recording functionality.
- **targetAgent**: The agent whose state and actions are tracked and recorded.
- **experimentData.recordEnable**: Controls whether data is actually written to disk.
- **experimentData.baseFolderName**: Sets the output directory for all exported CSV files.
- **experimentData.fileNamePrefix**: Sets the prefix for all exported data files, allowing for easy organization.

---

## Main Script Methods
- `Initialize(InteroceptiveAgent agent)`: Sets up the recorder and output directories based on config.
- `InitializeEpisode()`: Prepares data structures for a new episode.
- `RecordStep()`, `RecordStep(string action)`: Records a step of agent-environment interaction.
- `RecordFinalStep()`, `RecordFinalStep(string action)`: Records the final step of an episode.
- `RecordCollision()`, `RecordFoodConsumed()`, `RecordWaterConsumed()`, `RecordEvent(string eventType)`: Specialized event and state recorders.
- `ExportEpisodeSummary()`: Exports episode-level summary data to CSV.
- `CalculateFinalMetrics()`: Computes summary statistics for an episode.
- `SetResourceOffered(string resource)`, `RecordResourceChoice(string resourceChosen)`: Tracks resource choice events.
- `OnEpisodeBegin()`, `OnEpisodeEnd()`: Episode lifecycle hooks.
- `SetEpisodeEndType(string endType)`: Records the reason for episode termination.

---

## Practical Tips
- Always ensure `targetAgent` is assigned in the Inspector or via script.
- Use `experimentData.recordEnable` to toggle data collection for debugging or full experiments.
- Organize output files by setting `baseFolderName` and `fileNamePrefix` in your config.
- Use the provided methods to record custom events or resource choices for richer analysis.

---

## Further Details
- DataRecorder integrates with agent, environment, and event systems for complete experiment tracking.
- Output directory and file naming are managed via the main configuration file for reproducibility.
- For advanced usage, see code comments in `Assets/Scripts/Utility/DataRecorder.cs`. 