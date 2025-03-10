using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class HelperFunctions
{
    public static float Minimum(float input, float min)
    {
        if (input < min) return min;
        return input;
    }

    public static float Maximum(float input, float max)
    {
        if (input > max) return max;
        return input;
    }
}


