from enum import Enum, auto


class ActionType(Enum):
    """ActionType is an enum class that defines the action types.

    Attributes:
        LEFT (int): Left action type.
        RIGHT (int): Right action type.
        UP (int): Up action type.
        DOWN (int): Down action type.
        NONE (int): None action type.
    """

    NONE = 0
    FORWARD = auto()
    LEFT = auto()
    RIGHT = auto()
    EAT = auto()
    INCREASE_E = auto()
    INCREASE_T = auto()
    INCREASE_B = auto()


if __name__ == "__main__":
    print(ActionType.FORWARD)
    print(ActionType.FORWARD.value)
    print(ActionType.FORWARD.name)
