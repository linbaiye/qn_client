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

    public List<Tile> Neighbours(CreatureDirection direction)
    {
        if (direction == CreatureDirection.Up)
            return _upTiles;
        if (direction == CreatureDirection.Down)
            return _downTiles;
        if (direction == CreatureDirection.Left)
            return _leftTiles;
        if (direction == CreatureDirection.Right)
            return _rightTiles;
        return new List<Tile>();
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
    
    
    public Tile? Pick(CreatureDirection direction, Tile? up, Tile? down, Tile? left, Tile? right)
    {
        List<Tile> candidates = new List<Tile>();
        if (direction == CreatureDirection.Up)
            candidates = _upTiles;
        if (direction == CreatureDirection.Down)
            candidates = _downTiles;
        if (direction == CreatureDirection.Left)
            candidates = _leftTiles;
        if (direction == CreatureDirection.Right)
            candidates = _rightTiles;
        List<Tile> matched = new List<Tile>();
        foreach (var tile in candidates)
        {
            if (tile.MatchNeighbours(up, down, left, right))
            {
                matched.Add(tile);
            }
        }
        return PickRandom(matched);
    }


    public bool MatchNeighbours(Tile? up, Tile? down, Tile? left, Tile? right)
    {
        if (up != null && !_upTiles.Contains(up))
            return false;
        if (down != null && !_downTiles.Contains(down))
            return false;
        if (left != null && !_leftTiles.Contains(left))
            return false;
        if (right != null && !_rightTiles.Contains(right))
            return false;
        return true;
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