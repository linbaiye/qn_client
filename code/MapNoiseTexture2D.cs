using Godot;

namespace QnClient.code;

public partial class MapNoiseTexture2D : Node2D
{

    [Export] public NoiseTexture2D NoiseTexture;

    private Noise _noise;

    public override void _Ready()
    {
        _noise = NoiseTexture.Noise;
    }
    
    
}