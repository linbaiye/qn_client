using System;
using System.Collections.Generic;
using NLog;
using QnClient.code.entity;

namespace QnClient.code.util;

public class Tile(int id, int number)
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    private readonly List<Tile> _leftTiles = new();
    private readonly List<Tile> _rightTiles = new();
    private readonly List<Tile> _upTiles = new();
    private readonly List<Tile> _downTiles = new();

    public int Id { get; } = id;
    public int Number { get; } = number;


    private void Add(Tile tile, List<Tile> dir)
    {
        if (!dir.Contains(tile))
            dir.Add(tile);
        if (dir.Count > 1)
            Logger.Debug("Tile {} has neibhours.", tile);
    }
    public void AddNeighbour(Tile tile, CreatureDirection direction)
    {
        if (direction == CreatureDirection.Up)
            Add(tile, _upTiles);
        else if (direction == CreatureDirection.Down)
            Add(tile, _downTiles);
        else if (direction == CreatureDirection.Left)
            Add(tile, _leftTiles);
        else if (direction == CreatureDirection.Right)
            Add(tile, _rightTiles);
    }

    private Tile? PickRandom(List<Tile> tiles)
    {
        if (tiles.Count == 0)
            return null;
        var next = new Random().Next(0, tiles.Count);
        return tiles[next];
    }

    public Tile? PickRandom(CreatureDirection direction)
    {
        if (direction == CreatureDirection.Up)
            return PickRandom(_upTiles);
        if (direction == CreatureDirection.Down)
            return PickRandom(_downTiles);
        if (direction == CreatureDirection.Left)
            return PickRandom(_leftTiles);
        if (direction == CreatureDirection.Right)
            return PickRandom(_rightTiles);
        return null;
    }

    public override string ToString()
    {
        return Id + ":" + Number;
    }

    private bool Equals(Tile other)
    {
        return Id == other.Id && Number == other.Number;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Tile)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Number);
    }
}