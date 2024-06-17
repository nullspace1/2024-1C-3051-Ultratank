

using System;
using Microsoft.Xna.Framework;

public static class VectorUtils
{
    public static Vector3 GetRandomVec3Pos(Vector3 center, Random _rand = null)
    {
        Random rand = _rand ?? new();
        return center + new Vector3(rand.Next(-10000, 10000), 0, rand.Next(-10000, 10000));
    }
}