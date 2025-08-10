using System.Collections.Generic;
using Godot;

namespace QnClient.code.map;

public partial class OverGroundLayer:  AbstractGroundLayer
{
    public void CreateTileSet(IDictionary<int, Texture2D> tileIdTextures, AtzMapFileParser atzMapFileParser)
    {
        CreateTileSet(tileIdTextures, atzMapFileParser.TileOverIds);
    }
    

    protected override int GetTileId(AtzMapFileParser.MapCell cell)
    {
        return cell.TileOverId;
    }

    protected override int GetNumber(AtzMapFileParser.MapCell cell)
    {
        return cell.TileOverNumber;
    }
}