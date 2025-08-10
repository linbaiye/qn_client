using System.Collections.Generic;
using System.Linq;
using Godot;
using QnClient.code.util;

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
        ISet<Vector2I> painted = new HashSet<Vector2I>();
        atzMapFileParser.ForeachCell(start, end, (cell, x, y) =>
        {
            if (TileIdToSourceId.TryGetValue(cell.TileId, out var tileSourceId))
            {
                var coor = new Vector2I(x, y);
                painted.Add(coor);
                SetCell(coor, tileSourceId, new Vector2I(cell.TileNumber, 0));
            }
        });
    }

    public void DumpPattern(IDictionary<int, Texture2D> tileIdTextures, AtzMapFileParser atzMapFileParser)
    {
        ISet<int> sets = new HashSet<int>();
        TextureRect textureRect = new TextureRect();
        textureRect.SetSize(atzMapFileParser.End * VectorUtil.TileSize);
        //tileIdTextures.TryGetValue()
        //textureRect.DrawTextureRect();
        atzMapFileParser.ForeachCell(new Vector2I(30, 30), new Vector2I(50, 50), (cell, x, y) =>
        {
            var cells = GetUsedCells();
            //textureRect.DrawTextureRect();
            //GD.Print(new Vector2I(x, y) + ":" + cell.TileNumber);
            //sets.Add(cell.TileNumber);
        });
    }
}