using System.Collections.Generic;
using Godot;

namespace QnClient.code.map;

public partial class ObjectLayer : Node2D
{
	private readonly ISet<string> _animatedObjectSprites = new HashSet<string>();

	private IDictionary<int, MapObject> _idObjects = new Dictionary<int, MapObject>();
	private readonly ZipFileMapTextureLoader _textureLoader = ZipFileMapTextureLoader.Instance;

	public void Paint(string textureResourceName, AtzMapFileParser atzMapFileParser, Vector2I start, Vector2I end)
	{
        _idObjects = _textureLoader.LoadObjects(textureResourceName);
        foreach (var child in GetChildren())
        {
	        if (child is not AnimatedSprite2D animatedSprite2D  ||
	            string.IsNullOrEmpty(animatedSprite2D.Name) || 
	            !animatedSprite2D.Name.ToString().StartsWith("obj_"))
	        {
		        RemoveChild(child);
		        child.QueueFree();
	        }
        }
        atzMapFileParser.ForeachCell(start, end, (cell, x, y) => DrawObjectAtCoordinate(cell.ObjectId, x, y));
	}
	
	
	private AnimatedSprite2D CreateAnimatedSprite2d(MapObject mapObject, int x, int y)
	{
		SpriteFrames frames = new SpriteFrames();
		for (int i = 0; i < mapObject.Textures.Length; i++)
		{
			frames.AddFrame("default", mapObject.Textures[i]);
		}
		var ani = new AnimatedSprite2D()
		{
			Centered = false, Position = new Vector2(x * 32, y * 24), Offset = mapObject.Offset,
			SpriteFrames = frames,
			Autoplay = "default",
			Name = mapObject.Name(x, y),
		};
		return ani;
	}
	
	private void DrawObjectAtCoordinate(int objectId, int x, int y)
	{
		if (_idObjects.TryGetValue(objectId, out var objectInfo))
		{
			int xPos = x * 32;
			int yPos = y * 24;
			if (objectInfo.Textures.Length > 1)
			{
				if (!_animatedObjectSprites.Contains(objectInfo.Name(x, y)))
				{
					var animatedSprite2D = CreateAnimatedSprite2d(objectInfo, x, y);
					AddChild(animatedSprite2D);
					_animatedObjectSprites.Add(animatedSprite2D.Name);
				}
			}
			else
			{
				Sprite2D objectSprite = new Sprite2D()
				{
					Texture = objectInfo.Textures[0], Centered = false, Position = new Vector2(xPos, yPos),
					Offset = objectInfo.Offset,
				};
				AddChild(objectSprite);
			}
		}
	}
}