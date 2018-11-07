using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputStateUtils {
    // would like to use inlines here but can't since c#
    // Maybe I should switch to unreal :(
    //                                 | jump 1 - true, 0 - false
    //                                 || fire 1 - true, 0 - false
    //                                 || | left/right 00/11 - none, 01 - left, 10 - right
    //                                 || || fwd/back 00/11 - none, 01 - up, 10 - down
    //                                 vv vvvv
    // 0000 0000 0000 0000 0000 0000 0000 0000
    // 32 bits of input

    static uint ZAxisMask = 3; // 0011
    static int ZAxisShift = 0;

    static uint XAxisMask = 3 << 2; // 1100
    static int XAxisShift = 2;

    static uint FireMask = 1 << 4; // 10000
    static int FireMaskShift = 4;

    static uint JumpMask = 1 << 5;
    static int JumpMaskShift = 5;

    public static int GetZAxis(uint state) {
        state = state & ZAxisMask;
        if (state == 1)
        {
            return 1;
        }
        else if(state == 2){
            return -1;
        }
        return 0;
    }
    public static int GetXAxis(uint state) {
        state = state & XAxisMask;
        state = state >> XAxisShift;
        if (state == 1) {
            return 1;
        } else if (state == 2) {
            return -1;
        }
        return 0;
    }
    public static bool GetFireState(uint state) {
        state = state & FireMask;
        state = state >> FireMaskShift;
        if (state == 1)
        {
            return true;
        }
        else {
            return false;
        }
        /**
         * can possibly simplify to just return state;
         */
    }
    public static bool GetJumpState(uint state) {
        state = state & JumpMask;
        state = state >> JumpMaskShift;
        if (state == 1)
        {
            return true;
        }
        else {
            return false;
        }
    }

}
