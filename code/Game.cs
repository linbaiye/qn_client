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
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey key && key.Keycode == Key.M && key.Pressed)
        {
            _atzMap.Draw("small02", "south");
            // _overGroundLayer.DrawGround("small02", "south");
        }
    }
}