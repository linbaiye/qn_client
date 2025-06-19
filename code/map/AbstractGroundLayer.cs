using System.Collections.Generic;
using Godot;

namespace QnClient.code.map;

public abstract partial class AbstractGroundLayer : TileMapLayer
{
    protected IDictionary<int, int> TileIdToSourceId { get; } = new Dictionary<int,int>();

    protected void CreateTileSet(IDictionary<int, Texture2D> tileIdTextures, IEnumerable<int> tileIds)
    {
        TileIdToSourceId.Clear();
        foreach(var id in tileIds)
        {
            if (!tileIdTextures.TryGetValue(id, out var texture))
            {
                continue;
            }
            TileSetAtlasSource source = new TileSetAtlasSource() { Texture = texture , TextureRegionSize = new Vector2I(32, 24)};
            int width = texture.GetWidth() / 32;
            for (int w = 0; w < width; w++)
            {
                source.CreateTile(new Vector2I(w, 0));
            }
            int sourceId = TileSet.AddSource(source);
            TileIdToSourceId.TryAdd(id, sourceId);
        }
    }

    public abstract void Paint(IDictionary<int, Texture2D> tileIdTextures, AtzMapFileParser atzMapFileParser, Vector2I start, Vector2I end);
}