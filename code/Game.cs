using Godot;
using QnClient.code.map;

namespace QnClient.code;

public partial class Game : Node2D
{
    private AtzMap _atzMap;
    
    public override void _Ready()
    {
        var groundLayer = GetNode<GroundLayer>("GroundLayer");
        var overGroundLayer = GetNode<OverGroundLayer>("OverGroundLayer");
        var objectLayer = GetNode<ObjectLayer>("ObjectLayer");
        var rootLayer = GetNode<RoofLayer>("RoofLayer");
        _atzMap = new AtzMap(overGroundLayer, groundLayer, objectLayer, rootLayer);
        _atzMap.Draw("small02", "south");
    }

}