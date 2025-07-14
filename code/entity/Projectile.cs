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

    
    private void Init(Vector2 start, Vector2 end, Texture2D texture, int millis)
    {
        Texture = texture;
        //var distance = start.DistanceTo(end);
        Offset = new Vector2(0, Texture.GetSize().Y / 2);
        Position = start;
        //_lengthSeconds = distance * 0.15f;
        //_lengthSeconds = distance * 0.0015f;
        _lengthSeconds = (float)millis / 1000;
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
    
    /*public static Projectile Test(Vector2 start, Vector2 end)
    {
        var textures = SpriteLoader.Load("y1");
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://scene/projectile.tscn");
        var arrow = scene.Instantiate<Projectile>();
        arrow.Init(start, end, textures[20].Texture);
        return arrow;
    }*/
    
    public static Projectile Test(Vector2 start, Vector2 end, string sprite, int millis)
    {
        var textures = SpriteLoader.Load(sprite);
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://scene/projectile.tscn");
        var arrow = scene.Instantiate<Projectile>();
        arrow.Init(start, end, textures[20].Texture, millis);
        return arrow;
    }

    // public static Projectile Create(string id)
    // {
    //     var textures = SpriteLoader.Load("y2");
    // }
    
}