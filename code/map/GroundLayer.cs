using System.Collections.Generic;
using System.Linq;
using Godot;

namespace QnClient.code.map;

public partial class GroundLayer : AbstractGroundLayer
{
    public void CreateTileSet(IDictionary<int, Texture2D> tileIdTextures, AtzMapFileParser atzMapFileParser)
    {
        CreateTileSet(tileIdTextures, atzMapFileParser.TileIds);
    }
    
    public override void Paint(
        AtzMapFileParser atzMapFileParser,
        Vector2I start, Vector2I end)
    {
        ClearTiles();
        atzMapFileParser.ForeachCell(start, end, (cell, x, y) =>
        {
            if (TileIdToSourceId.TryGetValue(cell.TileId, out var tileSourceId))
            {
                SetCell(new Vector2I(x, y), tileSourceId, new Vector2I(cell.TileNumber, 0));
            }
        });
    }

    public void DumpPattern(AtzMapFileParser atzMapFileParser)
    {
        ISet<int> sets = new HashSet<int>();
        atzMapFileParser.ForeachCell(new Vector2I(30, 30), new Vector2I(50, 50), (cell, x, y) =>
        {
            GD.Print(new Vector2I(x, y) + ":" + cell.TileNumber);
            sets.Add(cell.TileNumber);
        });
        var orderedEnumerable = sets.OrderBy(i => i);
        foreach (var i in orderedEnumerable)
        {
            GD.Print(i);
        }
    }
}