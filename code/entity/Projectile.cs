using System.Collections.Generic;
using Godot;
using QnClient.code.sprite;
using QnClient.code.util;

namespace QnClient.code.entity;

public partial class Projectile: Sprite2D
{
    
    private static readonly ZipFileSpriteLoader SpriteLoader = ZipFileSpriteLoader.Instance;
    
    private Vector2 _velocity;

    private float _lengthSeconds;

    private float _elapsed;
    
    private float _speed;

    private static readonly Dictionary<CreatureDirection, Vector2> PlayerOffsetPosition = new()
    {
        { CreatureDirection.Up , new Vector2(6, -36) },
        { CreatureDirection.UpRight, new Vector2(24, -30) },
        { CreatureDirection.Right , new Vector2(29, -19) },
        { CreatureDirection.DownRight, new Vector2(19, -10) },
        { CreatureDirection.Down, new Vector2(25, 0) },
        { CreatureDirection.DownLeft, new Vector2(6, -9) },
        { CreatureDirection.Left, new Vector2(1, -21) },
        { CreatureDirection.UpLeft, new Vector2(12, -32) },
    };
    
    private void Init(Vector2 start, Vector2 end)
    {
        var distance = start.DistanceTo(end);
        Position = start;
        _lengthSeconds = distance * 0.15f;
        //_lengthSeconds = distance * 0.0015f;
        var rect = end - Position;
        Rotation = rect.Normalized().Angle();
        _velocity = rect / _lengthSeconds;
    }

    public override void _PhysicsProcess(double delta)
    {
        _elapsed += (float)delta;
        Position += (_velocity * (float)delta);
        if (_elapsed >= _lengthSeconds)
        {
            QueueFree();
        }
    }
    
    public static Projectile Test(Vector2 start, Vector2 end)
    {
        var textures = SpriteLoader.Load("y2");
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://scene/projectile.tscn");
        var arrow = scene.Instantiate<Projectile>();
        arrow.Init(start, end);
        arrow.Texture = textures[20].Texture;
        return arrow;
    }


    // public static Projectile Create(string id)
    // {
    //     var textures = SpriteLoader.Load("y2");
    // }
    
}