using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Flags]
public enum ConnectionSides
{
    None = 0,
    Up = 1,
    Right = 2,
    Down = 4,
    Left = 8,
}

public static class ConnectionSidesUtility
{
    public static readonly ConnectionSides[] All = { ConnectionSides.Up, ConnectionSides.Right, ConnectionSides.Down, ConnectionSides.Left };
    public static ConnectionSides Rotate(ConnectionSides sides, int steps)
    {
        steps = ((steps % 4) + 4) % 4;
        int mask = (int)sides & 15;
        mask = ((mask << steps) | (mask >> (4 - steps))) & 15;
        return (ConnectionSides)mask;
    }

    public static ConnectionSides Opposite(ConnectionSides side)
    {
        return Rotate(side, 2);
    }

    public static bool IsSingle(ConnectionSides side)
    {
        return Array.IndexOf(All, side) >= 0;
    }
    public static Vector2Int Offset(ConnectionSides side)
    {
        switch (side)
        {
            case ConnectionSides.Up: return new Vector2Int(0, -1);
            case ConnectionSides.Right: return new Vector2Int(1, 0);
            case ConnectionSides.Down: return new Vector2Int(0, 1);
            case ConnectionSides.Left: return new Vector2Int(-1, 0);
            default: return Vector2Int.zero;
        }
    }
}
