[![License: CC BY-NC-SA 4.0](https://img.shields.io/badge/License-CC%20BY--NC--SA%204.0-lightgrey.svg)](https://creativecommons.org/licenses/by-nc-sa/4.0/)
[![Unity 2022.3.16f1](https://img.shields.io/badge/Unity-2022.3.16f1-blue.svg)](https://unity.com/releases/editor/whats-new/2022.3.16)
[![C#](https://img.shields.io/badge/C%23-9.0-green.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)

# EVAAA: A Simulation Environment in Unity

## 📄 Table of Contents
- [EVAAA: A Simulation Environment in Unity](#evaaa-a-simulation-environment-in-unity)
  - [📄 Table of Contents](#-table-of-contents)
  - [📝 Overview](#-overview)
  - [🚀 Quickstart Example](#-quickstart-example)
  - [🛠️ Installation](#️-installation)
    - [1. Prerequisites](#1-prerequisites)
    - [2. Clone the Repository](#2-clone-the-repository)
    - [3. Open the Project in Unity](#3-open-the-project-in-unity)
    - [4. Configure Your Experiment](#4-configure-your-experiment)
    - [5. Run the Simulation](#5-run-the-simulation)
  - [🕹️ Usage](#️-usage)
    - [⚙️ Configuration System: Step-by-Step Guide](#️-configuration-system-step-by-step-guide)
      - [1. How the Config System Works](#1-how-the-config-system-works)
      - [2. Selecting an Experiment or Environment](#2-selecting-an-experiment-or-environment)
      - [3. Modifying Environment, Agent, or Task Settings](#3-modifying-environment-agent-or-task-settings)
      - [4. Example: Adding More Food](#4-example-adding-more-food)
      - [5. Creating a New Experiment](#5-creating-a-new-experiment)
      - [6. Tips and Troubleshooting](#6-tips-and-troubleshooting)
  - [📦 Project Structure](#-project-structure)
  - [🌍 Environments \& Tasks](#-environments--tasks)
    - [📈 Naturalistic Training Curriculum](#-naturalistic-training-curriculum)
    - [🧪 Experimental Testbeds](#-experimental-testbeds)
      - [Basic Homeostatic Regulation](#basic-homeostatic-regulation)
      - [Advanced Adaptive Skills](#advanced-adaptive-skills)
  - [📄 License](#-license)

---

## 📝 Overview
This folder contains the **Unity simulation environment** for EVAAA (Essential Variables in Autonomous and Adaptive Agents). The Unity environment is the core simulation backend for the EVAAA platform, providing a rich, extensible, and biologically inspired 3D world for reinforcement learning (RL) research.


Key features of the Unity environment:
- **Agent Embodiment & Multimodal Perception:** Agents are embodied in a 3D world and experience the environment through vision, olfaction, thermoception, collision detection, and interoception (internal physiological variables).
- **Unity ML-Agents Integration:** The environment is built on [Unity ML-Agents](https://github.com/Unity-Technologies/ml-agents), enabling seamless communication with RL training frameworks and supporting both AI and human-controlled agents.
- **Progressive Curriculum & Testbeds:** Includes a sequence of naturalistic training levels (curriculum) and a suite of controlled experimental testbeds for evaluating adaptive and generalizable behaviors.
- **JSON-Based Configuration System:** All aspects of the environment, agent, and tasks are controlled via editable JSON files—no code or scene changes required for new experiments or modifications.
- **Modular & Extensible Design:** The simulation is organized into modular components (Agent, Environment, Event System, Scene Controllers, UI, Utility), making it easy to extend or customize for new research needs.

This Unity project is intended for:
- Researchers designing new RL environments or experiments
- Developers extending the simulation or adding new features
- Anyone interested in embodied AI, biologically inspired RL, or Unity-based simulation

For training RL agents or running experiments, use this Unity environment in conjunction with the Python training suite (`evaaa_train`).

## 🚀 Quickstart Example

Follow these steps to launch your first EVAAA simulation:

1. **Clone the Repository**
   ```bash
   git lfs install
   git clone https://anonymous.4open.science/r/evaaa-2486
   cd evaaa
   ```

2. **Open the Project in Unity**
   - Launch **Unity Hub**.
   - Click **Add** and select the `evaaa_env` folder in the cloned folder.
   - Open the project using **Unity Editor 2022.3.16f1**.

3. **Select an Environment or Task**
   - Open `Config/mainConfig.json` in your preferred editor.
   - Set the `"configFolderName"` field to your desired experiment (e.g., `"train-level-1.1-ScatteredResource"`, `"exp-multiGoalPlanning"`).

4. **Run the Simulation**
   - In the Unity Editor, press the **Play** button.
   - Observe the agent interacting with the environment.

---

You can now explore, modify, or extend the EVAAA environments and tasks.

## 🛠️ Installation

Follow these detailed steps to set up Unity and run EVAAA:

### 1. Prerequisites
- **Unity Editor:** Install [Unity Hub](https://unity3d.com/get-unity/download) and use it to install **Unity Editor 2022.3.16f1** (LTS version). This is the only tested version for EVAAA.
- **Git:** Make sure you have [Git](https://git-scm.com/downloads) installed to clone the repository.

### 2. Clone the Repository
Open a terminal (Command Prompt, PowerShell, or Terminal) and run:
```bash
git lfs install
git clone https://anonymous.4open.science/r/evaaa-2486
cd evaaa
```

### 3. Open the Project in Unity
1. Launch **Unity Hub**.
2. Click **Add** (or the plus `+` icon) and select the `evaaa_evn` folder where you cloned.
3. In Unity Hub, make sure the project is set to use **Unity Editor 2022.3.16f1**. If not, click the three dots next to the project and select **Add Editor Version** to install it.
4. Click the project name to open it in Unity.

### 4. Configure Your Experiment
1. In your file browser or code editor, open `Config/mainConfig.json`.
2. Set the `"configFolderName"` field to the experiment you want to run (e.g., `"train-level-1.1-ScatteredResource"`, `"exp-multiGoalPlanning"`).
3. You can further customize agent, environment, and task parameters by editing the JSON files in the corresponding config folder.

### 5. Run the Simulation
1. In the Unity Editor, open the *MainSurvivalEnvironment* scene in the `Assets/Scene` folder (double-click to open).
2. Press the **Play** button at the top of the Unity Editor window.
3. All environment and task changes are controlled via the configuration files—there is no need to switch scenes.

**Manual Control:**
- In the Unity Editor, you can control the agent manually using your keyboard:
  - **Left:** ← (Left Arrow)
  - **Right:** → (Right Arrow)
  - **Forward:** ↑ (Up Arrow)
  - **Eat:** Spacebar
- This allows you to test and explore the environment as a human player.

---

**Tips for Beginners:**
- If you see any package or dependency errors, open the **Window > Package Manager** in Unity and let it resolve or install missing packages.
- For more information on Unity basics, see the [Unity Learn](https://learn.unity.com/) portal.
- You can always reset your configuration by restoring the original JSON files from the repository.

You are now ready to explore, modify, or extend EVAAA environments and tasks!

## 🕹️ Usage

### ⚙️ Configuration System: Step-by-Step Guide

EVAAA is designed so that all environment, agent, and task settings are controlled through easy-to-edit configuration (config) files. You do **not** need to change any code or Unity scenes to create new experiments or modify existing ones. This makes the system accessible even if you are new to Unity or programming.

#### 1. How the Config System Works
- The main config file (`Config/mainConfig.json`) tells EVAAA which experiment setup to use.
- Each experiment setup is a folder inside `Config/` (e.g., `train-level-1.1-ScatteredResource`, `train-level-2.1-Obstacles`).
- Each folder contains several JSON files, each controlling a different aspect of the environment (agent, resources, obstacles, etc.).
- When you run the simulation, EVAAA loads all settings from these files automatically.

#### 2. Selecting an Experiment or Environment
- Open `Config/mainConfig.json` in a text editor.
- Find the line: `"configFolderName": "train-level-1.1-ScatteredResource"` (the value may differ).
- Change the value to the name of any other folder in `Config/` to switch to a different environment or task.
- **Example:**
  ```json
  {
    "isAIControlled": true,
    "configFolderName": "train-level-2.1-Obstacles",
    ...
  }
  ```
- Save the file. The next time you press Play in Unity, the new environment will load.

**Pre-existing config folders you can use:**

| Survival Curriculum                        | Experimental Testbeds                |
|--------------------------------------------|--------------------------------------|
| train-level-1.1-Scattered-Cubes            | exp-multiGoalPlanning                |
| train-level-1.2-Corner-Located-Cubes       | exp-two-resource-food                |
| train-level-2.1-Natural-Obstacle-Layout    | exp-two-resource-water               |
| train-level-2.2-Obstacle-Resource-Pairings | exp-two-resource-thermo              |
| train-level-3.1-Navigation                 | exp-Ymaze                            |
| train-level-3.2-Exploration                | exp-goal-manipulation-FoodToWater    |
| train-level-4.1-Predator-Threat            | exp-goal-manipulation-WaterToFood    |
| train-level-4.2-Day-Night-Cycle            | exp-riskTaking                       |
|                                            | exp-collision-avoidance              |
|                                            | exp-predator                         |


![fig3](/image/fig3.png)

#### 3. Modifying Environment, Agent, or Task Settings
- Open the folder named in `configFolderName` (e.g., `Config/train-level-2.1-Obstacles/`).
- Each JSON file in this folder controls a different part of the simulation:
  - `agentConfig.json`: Agent movement, sensors, internal state, etc.
  - `resourceConfig.json`: Types, number, and placement of resources (food, water, etc.).
  - `obstacleConfig.json`: Types, number, and placement of obstacles (rocks, bushes, bonfires, etc.).
  - `daynightConfig.json`: Day/night cycle and lighting.
  - `thermoGridConfig.json`: Temperature grid settings.
  - ...and more.
- Open any of these files in a text editor. Change values as needed. **Examples:**
  - To add more food, increase the `count` in the `Food` group in `resourceConfig.json`.
  - To make the agent start with less health, lower the `startHealthLevel` in `agentConfig.json`.
  - To add obstacles, increase the `count` for rocks or bushes in `obstacleConfig.json`.
- Save your changes. Press Play in Unity to see the effect.

#### 4. Example: Adding More Food
Suppose you want to double the amount of food in the environment:
- Open `Config/train-level-1.1-ScatteredResource/resourceConfig.json`.
- Find the section:
  ```json
  {
    "prefabName": "Food",
    ...
    "count": 50,
    ...
  }
  ```
- Change `"count": 50` to `"count": 100`.
- Save the file and run the simulation.

#### 5. Creating a New Experiment
- Copy any existing config folder (e.g., `train-level-1.1-ScatteredResource`) and give it a new name (e.g., `my-custom-experiment`).
- Edit the JSON files in your new folder to set up your custom environment.
- In `Config/mainConfig.json`, set `"configFolderName"` to your new folder name.
- Save and run. You now have a new, reproducible experiment setup!

#### 6. Tips and Troubleshooting
- If you make a mistake in a JSON file, Unity may show an error or the simulation may not load as expected. Double-check your edits for typos or missing commas/brackets.
- You can always restore the original config files from your Git repository if needed.
- For more details on what each parameter means, see the documentation in the [DocumentationHierarchy](/evvva_unity/Documents/DocumentationHierarchy.md) folder.

---

By using the config system, you can easily explore, modify, and extend EVAAA's environments and tasks—no coding required!

## 📦 Project Structure

The EVAAA project is organized into several core script groups, each responsible for a key aspect of the simulation environment. For detailed documentation on each group, see the linked markdown files in the `Documents/` folder.

| Group | Description | Documentation |
|-------|-------------|---------------|
| 🤖 Agent Scripts | Core logic for agent behavior, perception, and interaction | [Agent.md](Documents/Agent/Agent.md) |
| 🌳 Environment Scripts | Procedural generation and management of environment | [Environment.md](Documents/Environment/Environment.md) |
| ⚡ Event System | In-game event management and triggers | [EventSystem.md](Documents/Event/EventSystem.md) |
| 🎬 Scene Controllers | Initialization and configuration of simulation | [SceneControllers.md](Documents/SceneControllers/SceneControllers.md) |
| 🖼️ User Interface (UI) | Visualization and control interfaces | [UI.md](Documents/UI/UI.md) |
| 🧰 Utility Scripts | General-purpose functions and tools | [Utility.md](Documents/Utility/Utility.md) |

---

**Note:** For each group, the corresponding markdown file in the [DocumentationHierarchy](/evaaa_unity/Documents/DocumentationHierarchy.md) folder provides a summary of the main scripts, their purposes, and key features. Subcomponents of the environment are also documented individually for clarity and modularity.

## 🌍 Environments & Tasks

At the heart of EVAAA is a unique integration of a two-tiered environment architecture with a unified, intrinsic reward system. In EVAAA, agents are motivated not by externally defined, task-specific rewards, but by the need to regulate their own internal physiological variables—such as food, water, thermal balance, and damage. This intrinsic reward system is consistent across all environments, allowing agents to autonomously generate and pursue goals that emerge from their internal state dynamics. The two-tiered design—combining a progressive, naturalistic survival curriculum with a suite of controlled, unseen experimental testbeds—enables both the development of adaptive behaviors and the evaluation of generalization and flexible decision-making in novel scenarios. By unifying these elements, EVAAA provides a biologically inspired benchmark for studying autonomy, adaptivity, and internally motivated control in reinforcement learning agents.

### 📈 Naturalistic Training Curriculum

EVAAA's training environments are structured as a sequence of four progressively challenging levels, each designed to scaffold adaptive survival behavior through internal-state regulation. Each level introduces new behavioral demands, environmental complexity, and learning opportunities:

| **Level** | **Name**                        | **Config Name**                        | **Description** |
|-----------|---------------------------------|----------------------------------------|-----------------|
| 1         | Basic Resource Foraging         | train-level-1.1-ScatteredResource      | Food and water are scattered randomly in an open field with changing temperatures. This level establishes basic foraging and consumption behaviors. |
|           |                                 | train-level-1.2-CorneredResource       | All resources are placed in a fixed corner, requiring the agent to navigate purposefully based on its needs. Both sublevels have no obstacles, focusing on essential variable (EV) regulation. |
| 2         | Obstacle-Resource Mapping       | train-level-2.1-Obstacles              | Bonfires add heat sources, so agents must manage their body temperature. Rocks and bushes block paths and can cause damage, introducing navigation and risk management. |
|           |                                 | train-level-2.2-ResourceMapping        | Resources are placed in consistent locations (e.g., food near trees, water in ponds), encouraging agents to learn and use environmental cues. More obstacles and cues increase complexity. |
| 3         | Terrain Exploration & Navigation| train-level-3.1-Navigation             | Tree areas are surrounded by walls, requiring agents to plan efficient routes while managing temperature and avoiding damage. |
|           |                                 | train-level-3.2-Exploration            | Only one tree area has food per episode, so agents must search and adapt their strategy as food locations change. Balancing multiple needs is key. |
| 4         | Dynamic Threatening Environment | train-level-4.1-Predator               | A predator is introduced, and agents must avoid it to survive, adding a threat-avoidance challenge. |
|           |                                 | train-level-4.2-DayAndNight            | The environment cycles between day and night, affecting temperature and predator activity. Agents must adapt their behavior to these changing conditions. |

---

### 🧪 Experimental Testbeds

EVAAA includes a suite of controlled testbed environments, each designed to isolate specific decision-making challenges and evaluate generalization. These are organized into two categories:

#### Basic Homeostatic Regulation
| **Category** | **Task Name**              | **Config Name**              | **Description** |
|--------------|---------------------------|------------------------------|-----------------|
| Basic        | Two-Resource Choice (Food) | exp-two-resource-food        | Agents choose between food and water, prioritizing based on low food levels. Tests if agents act according to internal needs. |
| Basic        | Two-Resource Choice (Water)| exp-two-resource-water       | Same as above, but agents start with low water levels, testing flexible prioritization. |
| Basic        | Two-Resource Choice (Thermo)| exp-two-resource-thermo     | Agents must regulate their body temperature by choosing the right resource. |
| Basic        | Avoiding Collision         | exp-riskTaking               | Agents must reach a resource while avoiding closely spaced obstacles, testing precise movement and damage avoidance. |

#### Advanced Adaptive Skills
| **Category** | **Task Name**                | **Config Name**                   | **Description** |
|--------------|-----------------------------|-----------------------------------|-----------------|
| Advanced     | Thermal Risk Task            | exp-riskTaking                   | A resource is behind a bonfire. Agents must decide whether to risk taking damage for a thermal reward. |
| Advanced     | Goal Manipulation (Food→Water) | exp-goal-manipulation-FoodToWater | When the agent crosses a transparent boundary, its essential variables change from low food/high water to low water/high food. The agent receives updated EV observations and must adapt its priorities accordingly. |
| Advanced     | Goal Manipulation (Water→Food) | exp-goal-manipulation-WaterToFood | Same as above, but the essential variables change from low water/high food to low food/high water. The agent receives updated EV observations and must adjust its behavior in response. |
| Advanced     | Spatial Navigation (Y-maze) | exp-Ymaze                         | Two resources are at the ends of a Y-shaped maze. Agents must revisit and change direction as their needs change. |
| Advanced     | Multi-Goal Planning         | exp-multiGoalPlanning             | Agents must prioritize and address multiple needs in sequence, testing their ability to coordinate and plan. |
| Advanced     | Predator Avoidance           | exp-predator                      | Agents must avoid predators during the day and forage at night, requiring adaptive, time-sensitive strategies. |

---

This curriculum and testbed suite enable systematic analysis of how agents learn, adapt, and generalize under internal-state-driven demands in complex, dynamic environments.


## 📄 License
CC BY-SA 4.0