using System;
using System.Collections.Generic;
using Godot;

namespace QnClient.code.map;

public partial class GroundLayer : AbstractGroundLayer
{
    public override void Paint(IDictionary<int, Texture2D> tileIdTextures,
        AtzMapFileParser atzMapFileParser,
        Vector2I start, Vector2I end)
    {
        CreateTileSet(tileIdTextures, atzMapFileParser.TileIds);
        atzMapFileParser.ForeachCell(start, end, (cell, x, y) =>
        {
            if (TileIdToSourceId.TryGetValue(cell.TileId, out var tileSourceId))
            {
                SetCell(new Vector2I(x, y), tileSourceId, new Vector2I(cell.TileNumber, 0));
            }
        });
    }
}