using System.Collections.Generic;
using Godot;

namespace QnClient.code.map;

public abstract partial class AbstractGroundLayer : TileMapLayer
{
    protected IDictionary<int, int> TileIdToSourceId { get; } = new Dictionary<int,int>();

    public override void _Ready()
    {
        //Modulate = new Color(1.0f, 1.0f, 1.0f, 0.7f);
    }


    /*public void DumpMap(TextureRect textureRect, AtzMapFileParser fileParser)
    {
        var size = new Vector2(1, 1) * VectorUtil.TileSize;
        textureRect.SetSize(size);
        var textureRectTexture = new CanvasTexture();
        textureRect.Texture = textureRectTexture;
        var texture = new ImageTexture();
            var empty = Image.CreateEmpty(size.X, size.Y, false, Image.Format.Rgb8);
            texture.SetImage(empty);
            textureRect.Texture = texture;
        fileParser.ForeachCell(Vector2I.Zero, fileParser.End, (cell, x, y) =>
        {
            if (x < 174|| x > 174||
               y < 221 || y > 221)
                return;
            var imageTexture = GetTileImage(new Vector2I(x, y));
            if (imageTexture == null)
                return;
            Rect2 rect2 = new Rect2(0, 0, VectorUtil.TileSize);
            textureRect.DrawTextureRect(imageTexture, rect2, false);
            imageTexture.GetImage().SavePng("res://" + x + "_" + y + ".png");
        });
        GD.Print(textureRect.Texture);
    }*/


    private ImageTexture? GetTileImage(Vector2I coordinate)
    {
        var cellSourceId = GetCellSourceId(coordinate);
        if (cellSourceId == -1)
            return null;
        var tileSetSource = (TileSetAtlasSource)GetTileSet().GetSource(cellSourceId);
        var altasCoord = GetCellAtlasCoords(coordinate);
        var tileTextureRegion = tileSetSource.GetTileTextureRegion(altasCoord);
        var image =  tileSetSource.Texture.GetImage();
        var region = image.GetRegion(tileTextureRegion);
        return ImageTexture.CreateFromImage(region);
    }


    protected void CreateTileSet(IDictionary<int, Texture2D> tileIdTextures, IEnumerable<int> tileIds)
    {
        TileIdToSourceId.Clear();
        foreach(var id in tileIds)
        {
            if (!tileIdTextures.TryGetValue(id, out var texture))
            {
                continue;
            }
            TileSetAtlasSource source = new TileSetAtlasSource() 
            {
                Texture = texture , 
                TextureRegionSize = new Vector2I(32, 24),
                UseTexturePadding = true,
            };
            int width = texture.GetWidth() / 32;
            for (int w = 0; w < width; w++)
            {
                source.CreateTile(new Vector2I(w, 0));
            }
            int sourceId = TileSet.AddSource(source);
            TileIdToSourceId.TryAdd(id, sourceId);
        }
    }

    protected void ClearTiles()
    {
        foreach (var usedCell in GetUsedCells())
        {
            EraseCell(usedCell);
        }
    }

    public abstract void Paint(AtzMapFileParser atzMapFileParser, Vector2I start, Vector2I end);
}