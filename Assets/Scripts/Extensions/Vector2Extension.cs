using System;
using UnityEngine;

public static class Vector2Extension
{
    public static bool IsVectorZero(this Vector2 vector2)
    {
        return vector2 == Vector2.zero;
    }
}
