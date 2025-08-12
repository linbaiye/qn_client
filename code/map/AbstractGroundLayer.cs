using System.Collections.Generic;
using System.Linq;
using Godot;

namespace QnClient.code.map;

public abstract partial class AbstractGroundLayer : TileMapLayer
{
    private IDictionary<int, int> TileIdToSourceId { get; } = new Dictionary<int,int>();
    
    private readonly ISet<Vector2I> _currentPainted = new HashSet<Vector2I>();

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

    private bool IsPainted(Vector2I point)
    {
        return _currentPainted.Contains(point);
    }
    

    private void EraseOrSavePaintedCells(ISet<Vector2I> newPainted)
    {
        ISet<Vector2I> removed = new HashSet<Vector2I>();
        foreach (var paintedCell in _currentPainted)
        {
            if (!newPainted.Contains(paintedCell))
            {
                EraseCell(paintedCell);   
                removed.Add(paintedCell);
            }
        }
        foreach (var vector2I in removed)
        {
            _currentPainted.Remove(vector2I);
        }
        foreach (var vector2I in newPainted)
        {
            _currentPainted.Add(vector2I);
        }
    }

    
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
    
    public void Paint(AtzMapFileParser atzMapFileParser,
        Vector2I start, Vector2I end)
    {
        ISet<Vector2I> painted = new HashSet<Vector2I>();
        atzMapFileParser.ForeachCell(start, end, (cell, x, y) =>
        {
            if (TileIdToSourceId.TryGetValue(GetTileId(cell), out var tileSourceId))
            {
                var coor = new Vector2I(x, y);
                painted.Add(coor);
                if (!IsPainted(coor))
                    SetCell(coor, tileSourceId, new Vector2I(GetNumber(cell), 0));
            }
        });
        EraseOrSavePaintedCells(painted);
    }

    protected abstract int GetTileId(AtzMapFileParser.MapCell cell);
    
    protected abstract int GetNumber(AtzMapFileParser.MapCell cell);


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

    public void ClearPaintedTiles()
    {
        _currentPainted.Clear();
        foreach (var usedCell in GetUsedCells())
        {
            EraseCell(usedCell);
        }
    }

}