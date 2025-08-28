using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using NLog;
using QnClient.code.entity;
using QnClient.code.map;

namespace QnClient.code.util;


public class TileAdjacency
{
    
    private readonly Dictionary<Tile, TileNeighbours> _tileNeighbours = new();
    
    private List<Tile> _tiles;
    

    public Tile Random()
    {
        // var rand = new Random();
        // var next = rand.Next(17, _tileAdjacency.Count);
        // return _tiles[next];
        return new Tile(12, 41);
    }
    
    public Dictionary<CreatureDirection, Tile> SelectNeighbours(Tile tile, Dictionary<CreatureDirection, Tile> currentNeighbours)
    {
        var ret = new Dictionary<CreatureDirection, Tile>();
        if (!_tileNeighbours.TryGetValue(tile, out var adjacency))
        {
            return ret;
        }
        return adjacency.SelectNeighbours(currentNeighbours);
    }


    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    
    public void Dump(AtzMapFileParser parser)
    {
        Dictionary<Vector2I, Tile> tiles = new();
        parser.ForeachCell(Vector2I.Zero, parser.End, (cell, x, y) =>
        {
            tiles.Add(new Vector2I(x, y), new Tile(cell.TileId, cell.TileNumber));
        });
        foreach (var (xy, tile) in tiles)
        {
            foreach (var dir in Enum.GetValues(typeof(CreatureDirection)).Cast<CreatureDirection>())
            {
                if (dir is CreatureDirection.Up or CreatureDirection.Down or CreatureDirection.Left or CreatureDirection.Right)
                {
                    var neighbour = xy.Move(dir);
                    if (!parser.IsInRange(neighbour.X, neighbour.Y))
                        continue;
                    if (!tiles.TryGetValue(neighbour, out var adjTile))
                        continue;
                    tile.AddNeighbour(adjTile, dir);
                }
            }
        }
        // StringBuilder stringBuilder = new StringBuilder();
        // foreach (var (tile, adjacency) in _tileAdjacency)
        // {
        //     if (tile.Id <= 10 || tile.Id >= 20)
        //         continue;
        //     foreach (var dir in Enum.GetValues(typeof(CreatureDirection)).Cast<CreatureDirection>())
        //     {
        //         stringBuilder.Append(tile + ":  " + adjacency.GetAdjacentTiles(dir)).Append('\n');
        //     }
        // }
        // FileAccess fileAccess = FileAccess.Open("res://structure.txt", FileAccess.ModeFlags.Write);
        // fileAccess.StoreString(stringBuilder.ToString());
        // fileAccess.Close();
    }
}