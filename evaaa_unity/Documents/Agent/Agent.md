# Agent System Overview

## What is the Agent System?
The Agent system in EVAAA defines how all agents—both learning agents and non-player agents like predators—sense, act, and adapt within the simulation. It is designed to be highly modular and configurable, so you can easily customize agent behavior, perception, and internal state dynamics for your experiments.

**Key features:**
- Modular components for different agent abilities (perception, movement, internal state, etc.)
- Nearly all agent behavior and sensing can be configured via simple JSON files—no coding required for most users
- Supports both AI-controlled and manually controlled agents

## Main Components
Each part of the agent system is documented in detail in its own markdown file:

- [**InteroceptiveAgent**](./InteroceptiveAgent.md): The main reinforcement learning agent, with internal physiological variables (food, water, thermal, health) and rich, animal-like perception. Highly configurable via JSON.
- [**Predator**](./Predator.md): Implements dynamic predator agents that act as moving threats, using a state machine and navigation logic.
- [**ObjectRaycast**](./ObjectRaycast.md): Provides obstacle and collision detection using a radial raycasting system, allowing agents to sense their surroundings.
- [**ThermalSensing**](./ThermalSensing.md): Enables agents to sense local environmental temperature using a spatial thermal grid.
- [**ResourceEating**](./ResourceEating.md): Manages how agents detect, consume, and gain benefit from resources like food, water, and ponds.
- [**ConfigurableCameraSensor**](./ConfigurableCameraSensor.md): Gives agents flexible, attachable camera vision for visual observations (RGB or grayscale), compatible with Unity ML-Agents.

For details on how to use and configure each component, see the linked documentation above. All components are designed to work together, making it easy to build, modify, and extend agents for a wide range of experiments in EVAAA. 