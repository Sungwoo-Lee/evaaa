from __future__ import annotations

from typing import Any, Dict

import time
import gymnasium as gym
from lightning import Fabric

from algos.dreamer_v3.agent import build_agent
from algos.dreamer_v3.utils import test
from utils.env import make_env
from utils.logger import get_log_dir, get_logger
from utils.registry import register_evaluation

# @register_evaluation(algorithms="dreamer_v3")
def evaluate(fabric: Fabric, cfg: Dict[str, Any], state: Dict[str, Any]):
    # logger = get_logger(fabric, cfg)
    # if logger and fabric.is_global_zero:
    #     fabric._loggers = [logger]
    #     fabric.logger.log_hyperparams(cfg)
    # log_dir = get_log_dir(fabric, cfg.root_dir, cfg.run_name)
    # fabric.print(f"Log dir: {log_dir}")
    log_dir = None

    env = make_env(
        cfg,
        cfg.seed,
        0,
        log_dir,
        "test",
        vector_env_idx=0,
    )()
    observation_space = env.observation_space
    action_space = env.action_space
    # env.close()
    # time.sleep(1)

    if not isinstance(observation_space, gym.spaces.Dict):
        raise RuntimeError(f"Unexpected observation type, should be of type Dict, got: {observation_space}")

    fabric.print("Encoder CNN keys:", cfg.algo.cnn_keys.encoder)
    fabric.print("Encoder MLP keys:", cfg.algo.mlp_keys.encoder)

    is_continuous = isinstance(action_space, gym.spaces.Box)
    is_multidiscrete = isinstance(action_space, gym.spaces.MultiDiscrete)
    actions_dim = tuple(
        action_space.shape if is_continuous else (action_space.nvec.tolist() if is_multidiscrete else [action_space.n])
    )
    # Create the actor and critic models
    _, _, _, _, player = build_agent(
        fabric,
        actions_dim,
        is_continuous,
        cfg,
        observation_space,
        state["world_model"],
        state["actor"],
    )
    del _
    test(env, player, fabric, cfg, log_dir, greedy=False)
