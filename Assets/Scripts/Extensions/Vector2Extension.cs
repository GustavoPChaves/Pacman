using UnityEngine;

public static class Vector2Extension
{
    public static bool IsVectorZero(this Vector2 vector2)
    {
        return vector2 == Vector2.zero;
    }

    public static bool IsVectorRight(this Vector2 vector2)
    {
        return vector2 == Vector2.right;
    }
    public static bool IsVectorLeft(this Vector2 vector2)
    {
        return vector2 == Vector2.left;
    }
    public static bool IsVectorUp(this Vector2 vector2)
    {
        return vector2 == Vector2.up;
    }
    public static bool IsVectorDown(this Vector2 vector2)
    {
        return vector2 == Vector2.down;
    }
    public static Vector2 FacingOrthogonalDirection(this Vector2 vector2)
    {
        if (vector2.IsVectorRight())
        {
            return Vector2.right;
        }
        if (vector2.IsVectorLeft())
        {
            return Vector2.left;
        }
        if (vector2.IsVectorUp())
        {
            return Vector2.up;

        }
        if (vector2.IsVectorDown())
        {
            return Vector2.down;
        }
        return Vector2.zero;
    }
}
