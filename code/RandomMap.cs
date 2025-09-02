using System.Collections.Generic;
using System.Linq;
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

    private readonly Dictionary<Tile, TileNeighbour> _tileNeighbours = new Dictionary<Tile, TileNeighbour>();
    
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

    private const int Width = 32;
    
    private const int Height = 32;
    

    private const int Size = 32;
    
    private void TileMap(Dictionary<Vector2I, Tile?> tiled, Vector2I xy, Tile? tile)
    {
        if (xy.X < 0 || xy.X > Size || xy.Y < 0 || xy.Y > Size)
            return;
        if (!tiled.TryAdd(xy, tile))
            return;
        if (tile == null)
            return;
        var sourceId = GetSourceId(tile.Id);
        SetCell(xy, sourceId, new Vector2I(tile.Number, 0));
        foreach (var dir in TileDirections)
        {
            var nextPos = xy.Move(dir);
            var up = tiled.GetValueOrDefault(nextPos.Move(CreatureDirection.Up), null);
            var down = tiled.GetValueOrDefault(nextPos.Move(CreatureDirection.Down), null);
            var left = tiled.GetValueOrDefault(nextPos.Move(CreatureDirection.Left), null);
            var right = tiled.GetValueOrDefault(nextPos.Move(CreatureDirection.Right), null);
            
            var nextTile = tile.Pick(dir, up, down, left, right);

            TileMap(tiled, nextPos, nextTile);
        }
    }
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
        Dictionary<Vector2I, Tile[]> coordinateAndTile = new();
        Tile empty = new Tile(0, 0);
        _atzMapFileParser.ForeachCell(Vector2I.Zero, _atzMapFileParser.End, (cell, x, y) =>
        {
            var vector2I = new Vector2I(x, y);
            Tile[] tmp = [empty, empty] ;
            if (cell.TileId != 0)
            {
                tmp[0] = new Tile(cell.TileId, cell.TileNumber);
                _tiles.Add(tmp[0]);
            }
            if (cell.TileOverId != 0)
            {
                tmp[1] = new Tile(cell.TileOverId, cell.TileOverNumber);
                _tiles.Add(tmp[1]);
            }
            foreach (var tile in _tiles)
            {
                if (tile.Id == cell.TileId && tile.Number == cell.TileNumber)
                {
                    tmp[0] = tile;
                }
                if (tile.Id == cell.TileOverId && tile.Number == cell.TileOverNumber)
                {
                    tmp[1] = tile;
                }
            }
            coordinateAndTile.Add(vector2I, tmp);
        });
        foreach (var (xy, tile) in coordinateAndTile)
        {
            foreach (var dir in TileDirections)
            {
                var neighbourXy = xy.Move(dir);
                if (!coordinateAndTile.TryGetValue(neighbourXy, out var adjTile))
                    continue;
                if (tile[0].Id != 0)
                {
                    if (!_tileNeighbours.ContainsKey(tile[0]))
                        _tileNeighbours.TryAdd(tile[0], new TileNeighbour());
                    var myNeighbour = _tileNeighbours.GetValueOrDefault(tile[0], null);
                    if (adjTile[0].Id != 0)
                        myNeighbour.AddNeighbour(adjTile[0], dir);
                    if (adjTile[1].Id != 0)
                        myNeighbour.AddNeighbour(adjTile[1], dir);
                }
                if (tile[1].Id != 0)
                {
                    if (!_tileNeighbours.ContainsKey(tile[1]))
                        _tileNeighbours.TryAdd(tile[1], new TileNeighbour());
                    var myNeighbour = _tileNeighbours.GetValueOrDefault(tile[1], null);
                    if (adjTile[0].Id != 0)
                        myNeighbour.AddNeighbour(adjTile[0], dir);
                    if (adjTile[1].Id != 0)
                        myNeighbour.AddNeighbour(adjTile[1], dir);
                }
            }
        }
    }

    public void Random()
    {
        CreateTileSet(_groundTextures, _atzMapFileParser.TileIds);
        Logger.Debug("Overtiles {}, tiles {}.", _atzMapFileParser.TileOverIds.Count(), _atzMapFileParser.TileIds.Count());
        foreach (var tileOverId in _atzMapFileParser.TileOverIds)
        {
            if (_atzMapFileParser.TileIds.Contains(tileOverId))
            {
                Logger.Debug("Tile id {} is tile and overtile.", tileOverId);
            }
        }
        DumpTiles();
        // Dictionary<Vector2I, Tile?> tiled = new Dictionary<Vector2I, Tile>();
        // foreach (Tile tile in _tiles)
        // {
        //     if (tile.Id == 12 && tile.Number == 41)
        //     {
        //         TileMap(tiled, Vector2I.Zero, tile);
        //         break;
        //     }
        // }
    }
}