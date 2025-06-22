#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using QnClient.code.entity;

namespace QnClient.code.map;

public class AtzMap(
    OverGroundLayer overGroundLayer,
    GroundLayer groundLayer,
    ObjectLayer objectLayer,
    RoofLayer roofLayer) : IMap
{
    private readonly ZipFileMapTextureLoader _textureLoader = ZipFileMapTextureLoader.Instance;

    private AtzMapFileParser? _fileParser;
    
	private readonly IDictionary<long, ISet<Vector2I>> _entityCoordinates= new Dictionary<long, ISet<Vector2I>>();

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
    
    public bool CanMove(Vector2I coordinate)
    {
        if (_fileParser == null || !_fileParser.CanMove(coordinate))
        {
            return false;
        }
        return !_entityCoordinates.Values.Any(s => s.Contains(coordinate));
    }


    public void Occupy(IEntity entity)
    {
        Free(entity);
        _entityCoordinates.TryAdd(entity.Id, new HashSet<Vector2I>() { entity.Coordinate });
    }


    public void Free(IEntity entity)
    {
        _entityCoordinates.Remove(entity.Id);
    }

    /*public void Occupy(GameDynamicObject dynamicObject)
    {
        _entityCoordinates.TryAdd(dynamicObject.Id, new HashSet<Vector2I>(dynamicObject.Coordinates));
    }
	
    public void Free(GameDynamicObject dynamicObject)
    {
        _entityCoordinates.Remove(dynamicObject.Id);
    }*/
}