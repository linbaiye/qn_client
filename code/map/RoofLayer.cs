using System.Collections.Generic;
using Godot;
using QnClient.code.util;

namespace QnClient.code.map;

public partial class RoofLayer : Node2D
{
	private IDictionary<int, MapObject> _mapRoofInfos = new Dictionary<int, MapObject>();
	
	private readonly ZipFileMapTextureLoader _textureLoader = ZipFileMapTextureLoader.Instance;

	public void Paint(string textureResourceName, AtzMapFileParser atzMapFileParser, Vector2I start, Vector2I end)
	{
		_mapRoofInfos = _textureLoader.LoadRoof(textureResourceName);
		foreach (var child in GetChildren())
		{
			RemoveChild(child);
			child.QueueFree();
		}
		atzMapFileParser.ForeachCell(start, end, (cell, x, y) => DrawRoofAtCoordinate(cell.RoofId, x, y));
	}
	
	private void DrawRoofAtCoordinate(int rofId, int x, int y)
	{
		if (_mapRoofInfos.TryGetValue(rofId, out var rofInfo))
		{
			int xPos = x * VectorUtil.TileSizeX;
			int yPos = y * VectorUtil.TileSizeY;
			Sprite2D objectSprite = new Sprite2D()
			{
				Texture = rofInfo.Textures[0], Centered = false, Position = new Vector2(xPos, yPos),
				Offset = rofInfo.Offset,
			};
			AddChild(objectSprite);
		}
	}
}