# Environment System Overview

## What is the Environment System?
The Environment system in EVAAA defines the world in which agents operate. It is designed to be highly modular and configurable, so you can easily build, modify, and extend environments for a wide range of experiments—from simple temperature regulation to complex predator-prey and multi-agent scenarios.

**Key features:**
- Modular components for different environment features (temperature, arena, obstacles, resources, predators)
- Nearly all environment features can be configured via simple JSON files—no coding required for most users
- Supports reproducible research and systematic parameter sweeps

## Main Components
Each part of the environment system is documented in detail in its own markdown file:

- [**ThermoGridSpawner**](./ThermoGridSpawner_Introduction.md): Manages temperature dynamics in the environment using a spatial thermal grid.
- [**CourtSpawner**](./CourtSpawner_Introduction.md): Generates the physical arena or court for experiments.
- [**ObstacleSpawner**](./ObstacleSpawner_Introduction.md): Places obstacles (e.g., rocks, bushes, bonfires) with optional thermal properties.
- [**ResourceSpawner**](./ResourceSpawner_Introduction.md): Manages consumable resources (food, water, ponds) and their placement.
- [**PredatorSpawner**](./PredatorSpawner_Introduction.md): Implements adversarial entities (predators) that challenge agents.

All components are highly configurable through their respective JSON files, allowing you to create custom environments and run reproducible experiments.

## Research Applications & Benchmarks
The EVAAA environment system supports a variety of research scenarios, including:
- Homeostatic reinforcement learning
- Physiologically-motivated reward functions
- Embodied intelligence with internal state regulation
- Multi-agent cooperation and resource sharing
- Interoceptive awareness and adaptive decision-making

**Benchmark scenarios include:**
1. Temperature Regulation
2. Resource Gathering
3. Predator Avoidance
4. Multi-Agent Cooperation

## RL Integration & Observations
- Standardized observation and action spaces compatible with RL frameworks
- Agent observations include internal state, local environment, and (optionally) global state
- Actions include movement, resource interaction, and (optionally) communication
- Rewards can be based on homeostasis, resource collection, survival, or custom functions

## Reproducible Research
All environment configurations are saved as JSON files in your experiment folder, e.g.:
```
{environment_name}/{experiment_id}/
  ├── thermoGridConfig.json
  ├── courtConfig.json  
  ├── obstacleConfig.json
  ├── resourceConfig.json
  └── predatorConfig.json
```

## Getting Started
For details on how to use and configure each component, see the linked documentation above. All components are designed to work together, making it easy to build, modify, and extend environments for your research in EVAAA. 