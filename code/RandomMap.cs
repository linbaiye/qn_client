using System.Collections.Generic;
using Godot;
using NLog;
using QnClient.code.entity;
using QnClient.code.map;
using QnClient.code.util;

namespace QnClient.code;

public partial class RandomMap : AbstractGroundLayer
{
    private IDictionary<int, Texture2D> _groundTextures;
    private AtzMapFileParser _atzMapFileParser;
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private ISet<Tile> _tiles = new HashSet<Tile>();


    private static readonly CreatureDirection[] TileDirections = [CreatureDirection.Left, CreatureDirection.Right, CreatureDirection.Up, CreatureDirection.Down
    ];
    
    public override void _Ready()
    {
        _atzMapFileParser = AtzMapFileParser.ParseFile("res://maps/start.map");
        _groundTextures = ZipFileMapTextureLoader.Instance.LoadTiles("start");
        Random();
    }

    protected override int GetTileId(AtzMapFileParser.MapCell cell)
    {
        return cell.TileId;
    }

    protected override int GetNumber(AtzMapFileParser.MapCell cell)
    {
        return cell.TileNumber;
    }

    
    private const int Size = 30;
    private void TileMap(ISet<Vector2I> tiled, Vector2I xy, Tile? tile)
    {
        if (xy.X < 0 || xy.X > Size || xy.Y < 0 || xy.Y > Size)
            return;
        if (!tiled.Add(xy))
            return;
        if (tile == null)
            return;
        var sourceId = GetSourceId(tile.Id);
        SetCell(xy, sourceId, new Vector2I(tile.Number, 0));
        foreach (var dir in TileDirections)
        {
            var vector2I = xy.Move(dir);
            var next = tile.PickRandom(dir);
            TileMap(tiled, vector2I, next);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Pressed)
        {
            ClearPaintedTiles();
            Random();
        }
    }

    private void DumpTiles()
    {
        Dictionary<Vector2I, Tile> coordinateAndTile = new();
        _atzMapFileParser.ForeachCell(Vector2I.Zero, _atzMapFileParser.End, (cell, x, y) =>
        {
            coordinateAndTile.Add(new Vector2I(x, y), new Tile(cell.TileId, cell.TileNumber));
        });
        foreach (var (xy, tile) in coordinateAndTile)
        {
            foreach (var dir in TileDirections)
            {
                var neighbour = xy.Move(dir);
                if (!_atzMapFileParser.IsInRange(neighbour.X, neighbour.Y))
                    continue;
                if (!coordinateAndTile.TryGetValue(neighbour, out var adjTile))
                    continue;
                tile.AddNeighbour(adjTile, dir);
            }
        }
        _tiles = new HashSet<Tile>(coordinateAndTile.Values);
    }

    public void Random()
    {
        CreateTileSet(_groundTextures, _atzMapFileParser.TileIds);
        DumpTiles();
        ISet<Vector2I> tiled = new HashSet<Vector2I>();
        foreach (Tile tile in _tiles)
        {
            if (tile.Id == 12 && tile.Number == 41)
            {
                TileMap(tiled, Vector2I.Zero, tile);
                break;
            }
        }
    }
}