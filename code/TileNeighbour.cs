using System;
using System.Collections.Generic;
using QnClient.code.entity;
using QnClient.code.util;

namespace QnClient.code;

public class TileNeighbour
{
    private readonly List<Tile> _leftTiles = new();
    private readonly List<Tile> _rightTiles = new();
    private readonly List<Tile> _upTiles = new();
    private readonly List<Tile> _downTiles = new();
    
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
}