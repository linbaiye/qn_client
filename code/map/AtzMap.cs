#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using NLog;
using QnClient.code.entity;
using QnClient.code.entity.@event;
using QnClient.code.player.character;

namespace QnClient.code.map;

public class AtzMap(
    OverGroundLayer overGroundLayer,
    GroundLayer groundLayer,
    ObjectLayer objectLayer,
    RoofLayer roofLayer) : IMap
{
    private readonly ZipFileMapTextureLoader _textureLoader = ZipFileMapTextureLoader.Instance;

    private AtzMapFileParser? _fileParser;
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
    
	private readonly IDictionary<long, ISet<Vector2I>> _entityCoordinates= new Dictionary<long, ISet<Vector2I>>();

    private IDictionary<int, Texture2D> _groundTextures;

    private string _resourceName;

    public void Load(string name, string textureResourceName)
    {
        if (!textureResourceName.Equals(_resourceName))
        {
            _groundTextures = _textureLoader.LoadTiles(textureResourceName);
            _resourceName = textureResourceName;
        }
        _entityCoordinates.Clear();
        if (_fileParser == null || !_fileParser.Name.Equals(name))
        {
            _fileParser = AtzMapFileParser.ParseFile("res://maps/" + name + ".map");
            if (_fileParser == null)
            {
                throw new Exception("Can't load map file + " + name);
            }
            groundLayer.CreateTileSet(_groundTextures, _fileParser);
            overGroundLayer.CreateTileSet(_groundTextures, _fileParser);
        }
        objectLayer.LoadTextures(textureResourceName);
        roofLayer.LoadTextures(textureResourceName);
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

    private void Free(IEntity entity)
    {
        _entityCoordinates.Remove(entity.Id);
    }

    private void Draw(Vector2I coordinate)
    {
        var start = new Vector2I(Math.Max(coordinate.X - 20, 0), Math.Max(0, coordinate.Y - 20));
        var end = new Vector2I(Math.Min(coordinate.X + 30, _fileParser.End.X), Math.Min(coordinate.Y + 30, _fileParser.End.Y));
        groundLayer.Paint(_fileParser, start, end);
        overGroundLayer.Paint(_fileParser, start, end);
        objectLayer.Paint(_fileParser, start, end);
        roofLayer.Paint(_fileParser, start, end);
    }

    public void HandleEntityEvent(IEntityEvent entityEvent)
    {
        if (entityEvent is EntityChangeCoordinateEvent movementEvent)
        {
            if (movementEvent.Source is DynamicObject dynamicObject)
                Occupy(dynamicObject);
            else
                Occupy(movementEvent.Source);
            if (entityEvent.Source is not Character c)
            {
                return;
            }
            Draw(c.Coordinate);
            if (_fileParser.ShouldHideRoof(c.Coordinate))
            {
                roofLayer.Hide();
            } 
            else if (!roofLayer.Visible)
            {
                roofLayer.Show();
            }
        }
        else if (entityEvent is DeletedEvent)
        {
            if (entityEvent.Source is DynamicObject dynamicObject)
                Free(dynamicObject);
            else
                Free(entityEvent.Source);
        }
        else if (entityEvent is LiftCoordinatesEvent)
        {
            if (entityEvent.Source is DynamicObject dynamicObject)
                Free(dynamicObject);
        }
    }
    
    public Vector2I Start => Vector2I.Zero;
    public Vector2I End => _fileParser.End;

    private void Occupy(DynamicObject dynamicObject)
    {
        Free(dynamicObject);
        _entityCoordinates.TryAdd(dynamicObject.Id, new HashSet<Vector2I>(dynamicObject.Coordinates));
    }

    private void Free(DynamicObject dynamicObject)
    {
        _entityCoordinates.Remove(dynamicObject.Id);
    }
}