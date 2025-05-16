# User Interface (UI) Overview

The UI scripts in EVAAA provide real-time visualization and control interfaces for agents, environment states, and experiment feedback. These components enhance interpretability, debugging, and user interaction during simulation runs.

## Main Components

### ResourceUI.cs
- **Purpose:** Displays the agent's internal state variables (food, water, thermal, health) using sliders and color-coded handles.
- **Key Features:**
  - Real-time updates of resource levels.
  - Visual differentiation of each essential variable.

### TotalRewardText.cs
- **Purpose:** Shows the agent's current and average reward, along with the episode number.
- **Key Features:**
  - Updates reward display dynamically.
  - Optionally color-codes text based on reward value.

### RadialMeterController.cs
- **Purpose:** Visualizes essential variables as radial meters, indicating deviation from optimal set-points.
- **Key Features:**
  - Smooth transitions for fill amount and color.
  - Configurable thresholds for warning and critical states.

### ThermoceptionUI.cs
- **Purpose:** Displays a grid-based visualization of the agent's thermal perception.
- **Key Features:**
  - Color-codes grid cells based on temperature deviation from set-point.
  - Optionally displays temperature values as text.

### CameraSwitcher.cs
- **Purpose:** Allows toggling between first-person and third-person camera views, adjusting UI panels accordingly.
- **Key Features:**
  - Supports both AI and human control modes.
  - Manages visibility of UI elements based on view.

### HeatMap.cs
- **Purpose:** Renders a heatmap of environmental temperature, synchronized with the thermal grid.
- **Key Features:**
  - Updates texture and color gradient based on environment state.
  - Optionally tracks agent position on the map.

### AgentTrackBlackDot.cs
- **Purpose:** Tracks and displays the agent's position as a dot on the heatmap UI.
- **Key Features:**
  - Converts agent world position to UI coordinates.
  - Updates dot position in real time.

### AgentFollowCamera.cs
- **Purpose:** Controls a camera that follows the agent, with configurable position and angle.
- **Key Features:**
  - Loads camera settings from configuration files.
  - Smoothly updates camera position relative to the agent.

### UIPositioner.cs
- **Purpose:** Dynamically positions and scales UI elements (e.g., faces) relative to radial meters.
- **Key Features:**
  - Adjusts UI layout based on screen and element sizes.

---

These UI scripts collectively provide a rich, interactive interface for monitoring agent state, environment dynamics, and experiment progress in EVAAA. 