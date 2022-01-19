using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    
    public static float Map(float v, float a, float b, float x, float y)
    {
        return ((v - a) / (b - a)) * (y - x) + x;
    }

    public static Vector3 ToVector3(this Vector3Int v)
    {
        return new Vector3(v.x, v.y, v.z);
    }

}
