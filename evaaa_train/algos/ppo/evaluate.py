from __future__ import annotations

from typing import Any, Dict

import gymnasium as gym
from lightning import Fabric

from algos.ppo.agent import build_agent
from algos.ppo.utils import test
from utils.env import make_env
from utils.logger import get_log_dir, get_logger
# from utils.registry import register_evaluation


# @register_evaluation(algorithms="ppo")
# def evaluate_ppo(fabric: Fabric, cfg: Dict[str, Any], state: Dict[str, Any]):
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

    if not isinstance(observation_space, gym.spaces.Dict):
        raise RuntimeError(f"Unexpected observation type, should be of type Dict, got: {observation_space}")
    # if cfg.algo.cnn_keys.encoder + cfg.algo.mlp_keys.encoder == []:
    #     raise RuntimeError(
    #         "You should specify at least one CNN keys or MLP keys from the cli: "
    #         "`cnn_keys.encoder=[rgb]` or `mlp_keys.encoder=[state]`"
    #     )
    fabric.print("Encoder CNN keys:", cfg.algo.cnn_keys.encoder)
    fabric.print("Encoder MLP keys:", cfg.algo.mlp_keys.encoder)

    is_continuous = isinstance(env.action_space, gym.spaces.Box)
    is_multidiscrete = isinstance(env.action_space, gym.spaces.MultiDiscrete)
    actions_dim = tuple(
        env.action_space.shape
        if is_continuous
        else (env.action_space.nvec.tolist() if is_multidiscrete else [env.action_space.n])
    )
    # Create the actor and critic models
    _, agent = build_agent(fabric, actions_dim, is_continuous, cfg, observation_space, state["agent"])
    del _
    test(env, agent, fabric, cfg, log_dir)


# # This is just for showcase
# @register_evaluation(algorithms="ppo_decoupled")
# def evaluate_ppo_decoupled(fabric: Fabric, cfg: Dict[str, Any], state: Dict[str, Any]):
#     evaluate_ppo(fabric, cfg, state)
