#nullable enable
using System;
using System.Collections.Generic;
using Godot;

namespace QnClient.code.map;

public class AtzMap(
    OverGroundLayer overGroundLayer,
    GroundLayer groundLayer,
    ObjectLayer objectLayer,
    RoofLayer roofLayer)
{
    private readonly ZipFileMapTextureLoader _textureLoader = ZipFileMapTextureLoader.Instance;

    private AtzMapFileParser? _fileParser;

    public void Draw(string name, string textureResourceName)
    {
        if (_fileParser != null && !_fileParser.Name.Equals(name))
            return;
        _fileParser = AtzMapFileParser.ParseFile("res://maps/" + name + ".map");
        if (_fileParser == null)
        {
            throw new Exception("Can't load map file + " + name);
        }
        IDictionary<int,Texture2D> texture2Ds = _textureLoader.LoadTiles(textureResourceName);
        groundLayer.Paint(texture2Ds, _fileParser, Vector2I.Zero, _fileParser.End);
        overGroundLayer.Paint(texture2Ds, _fileParser, Vector2I.Zero, _fileParser.End);
        objectLayer.Paint(textureResourceName, _fileParser, Vector2I.Zero, _fileParser.End);
        roofLayer.Paint(textureResourceName, _fileParser, Vector2I.Zero, _fileParser.End);
    }
}