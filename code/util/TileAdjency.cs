namespace QnClient.code.util;


public class TileAdjacency
{
    
    private readonly Dictionary<Direction, List<Tile>> _adjacency = new();


    public void Add(Direction direction, Tile tile)
    {
        if (_adjacency.TryGetValue(direction, out var adjacentTiles))
        {
            if (!adjacentTiles.Contains(tile))
                adjacentTiles.Add(tile);
        }
        else
        {
            _adjacency.Add(direction, new List<Tile> { tile });
        }
    }

    public string GetAdjacentTiles(Direction direction)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(direction.ToString());
        stringBuilder.Append("   ");
        if (_adjacency.TryGetValue(direction, out var adjacentTiles))
        {
            foreach (var tile in adjacentTiles)
            {
                stringBuilder.Append(tile);
                stringBuilder.Append(", ");
            }
        }
        return stringBuilder.ToString();
    }
    
    public void Dump()
    {
        ForeachCell(Vector2I.Zero, End, (cell, x, y) =>
        {
            _tiles.Add(new Vector2I(x, y), new Tile(cell.TileId, cell.TileNumber));
        });
        foreach (var (origin, originTile) in _tiles)
        {
            foreach (var dir in Enum.GetValues(typeof(Direction)).Cast<Direction>())
            {
                var neighbour = origin.Move(dir);
                if (!IsInRange(neighbour.X, neighbour.Y))
                    continue;
                if (!_tiles.TryGetValue(neighbour, out var adjTile))
                    continue;
                if (_tileAdjacencies.TryGetValue(originTile, out var adj))
                {
                    adj.Add(dir, adjTile);
                }
                else
                {
                    var tileAdjacency = new TileAdjacency();
                    tileAdjacency.Add(dir, adjTile);
                    _tileAdjacencies.Add(originTile, tileAdjacency);
                }
            }
        }
        foreach (var (tile, adjacency) in _tileAdjacencies)
        {
            if (tile.TileId >= 20)
                continue;
            foreach (var dir in Enum.GetValues(typeof(Direction)).Cast<Direction>())
            {
                GD.Print(tile + ":  ", adjacency.GetAdjacentTiles(dir));
            }
        }
    }
    
}
public class TileAdjency
{
    
}