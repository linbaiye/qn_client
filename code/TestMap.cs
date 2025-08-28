using Godot;

namespace QnClient.code;

public partial class TestMap : Node2D
{

    [Export]
    public NoiseTexture2D NoiseTexture;

    private TileMapLayer _tileMapLayer;
    private Sprite2D _sprite;

    public override void _Ready()
    {
        _tileMapLayer = GetNode<TileMapLayer>("Map");
        _sprite = GetNode<Sprite2D>("");
        Tile();
    }

    private int NoiseToSourceId(float noise)
    {
        if (noise < 0.1f)
            return 63;
        if (noise <= 0.15f)
            return 70;
        return 96;
    }

    private void Tile()
    {
        var noise = NoiseTexture.Noise;
        for (int i = 0; i < 32; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                var noise2D = noise.GetNoise2D(i, j);
                var id = NoiseToSourceId(noise2D);
                _tileMapLayer.SetCell(new Vector2I(i, j), id, new Vector2I(0, 0));
            }
        }
    }
}