using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Movement
{
    public static MovementState GetMovementState(float input, MovementType movementType)
    {
        MovementState state = MovementState.IDLE;

        if (movementType == MovementType.FORWARD)
        {
            if (input > 0)
                state = MovementState.FRONT;
            else if (input < 0)
                state = MovementState.BACK;
        }

        if (movementType == MovementType.SIDE)
        {
            if (input > 0)
                state = MovementState.RIGHT;
            else if (input < 0)
                state = MovementState.LEFT;
        }

        return state;
    }

    public static bool IsIdle(MovementState state)
    {
        if (state == MovementState.IDLE)
            return true;

        return false;
    }

    public static bool IsNotIdle(MovementState state)
    {
        if (state != MovementState.IDLE)
            return true;

        return false;
    }
}

