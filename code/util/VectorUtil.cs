using System;
using System.Collections.Generic;
using Godot;
using QnClient.code.entity;
using QnClient.code.player;

namespace QnClient.code.util;

public static class VectorUtil
{
    public const int TileSizeX = 32;
    public const int TileSizeY = 24;
    public static readonly Vector2I TileSize = new(32, 24);
    
    public static readonly Vector2 DefaultTextureOffset = new (16, -12);


    private static readonly IDictionary<CreatureDirection, Vector2> VelocityMap = new Dictionary<CreatureDirection, Vector2>()
    {
        {CreatureDirection.Right, new Vector2(TileSizeX, 0)},
        {CreatureDirection.DownRight, new Vector2(TileSizeX, TileSizeY)},
        {CreatureDirection.Down, new Vector2(0, TileSizeY)},
        {CreatureDirection.DownLeft, new Vector2(-TileSizeX, TileSizeY)},
        {CreatureDirection.Left, new Vector2(-TileSizeX, 0)},
        {CreatureDirection.UpLeft, new Vector2(-TileSizeX, -TileSizeY)},
        {CreatureDirection.Up, new Vector2(0, -TileSizeY)},
        {CreatureDirection.UpRight, new Vector2(TileSizeX, -TileSizeY)},
    };


    public static Vector2 VelocityUnit(CreatureDirection direction)
    {
        return VelocityMap.TryGetValue(direction, out var ret) ? ret : Vector2.Zero;
    }


    public static Vector2I ToCoordinate(this Vector2 vector2)
    {
        return new Vector2I((int)(vector2.X / TileSizeX), (int)(vector2.Y / TileSizeY));
    }

    public static Vector2I Move(this Vector2I src, CreatureDirection direction)
    {
        return direction switch
        {
            CreatureDirection.Up => new Vector2I(src.X, src.Y - 1),
            CreatureDirection.Down => new Vector2I(src.X, src.Y + 1),
            CreatureDirection.Left => new Vector2I(src.X - 1, src.Y),
            CreatureDirection.Right => new Vector2I(src.X + 1, src.Y),
            CreatureDirection.UpRight => new Vector2I(src.X + 1, src.Y - 1),
            CreatureDirection.DownRight=> new Vector2I(src.X + 1, src.Y + 1),
            CreatureDirection.DownLeft=> new Vector2I(src.X - 1, src.Y + 1),
            CreatureDirection.UpLeft => new Vector2I(src.X - 1, src.Y - 1),
            _ => src
        };
    }

    public static int Distance(this Vector2I src, Vector2I dst)
    {
        return Math.Max(Math.Abs(src.X - dst.X), Math.Abs(src.Y - dst.Y));
    }

        
    public static Vector2I Move(this Vector2I src, int x, int y)
    {
        return new Vector2I(src.X + x, src.Y + y);
    }

    public static Vector2 ToPosition(this Vector2I vector2)
    {
        return new Vector2(vector2.X * TileSizeX, vector2.Y * TileSizeY);
    }

    public static CreatureDirection GetDirection(this Vector2I src, Vector2I another)
    {
        var p1 = src.ToPosition();
        var p2 = another.ToPosition();
        return (p2 - p1).GetDirection();
    }

    public static CreatureDirection GetDirection(this Godot.Vector2 vector)
    {
        var angle = Mathf.Snapped(vector.Angle(), Mathf.Pi / 4) / (Mathf.Pi / 4);
        int dir = Mathf.Wrap((int)angle, 0, 8);
        return dir switch
        {
            0 => CreatureDirection.Right,
            1 => CreatureDirection.DownRight,
            2 => CreatureDirection.Down,
            3 => CreatureDirection.DownLeft,
            4 => CreatureDirection.Left,
            5 => CreatureDirection.UpLeft,
            6 => CreatureDirection.Up,
            7 => CreatureDirection.UpRight,
            _ => CreatureDirection.Right,
        };
    }
    
    private static readonly Dictionary<MoveAction, float> MoveStateSeconds = new()
    {
        { MoveAction.Walk, 0.84f },
        { MoveAction.FightWalk, 0.84f },
        { MoveAction.Run, 0.42f },
        { MoveAction.Fly, 0.36f },
    };

    public static float GetMoveDuration(MoveAction action)
    {
        if (MoveStateSeconds.TryGetValue(action, out var value))
            return value;
        // should never happen.
        throw new NotSupportedException();
    }

}