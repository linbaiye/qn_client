using Godot;
using QnClient.code.sprite;

namespace QnClient.code.entity;

public partial class Projectile: AnimatedSprite2D
{
    
    private static readonly ZipFileSpriteLoader SpriteLoader = ZipFileSpriteLoader.Instance;
    
    private Vector2 _velocity;

    private float _lengthSeconds;

    private float _elapsed;
    
    private float _speed;

    
    private void Init(Vector2 start, Vector2 end, OffsetTexture[] textures, int millis)
    {
        
        SpriteFrames frames = new SpriteFrames();
        for (int i = 0; i < 10; i++)
        {
            frames.AddFrame("default", textures[20 + i].Texture);
        }
        Autoplay = "default";
        SpriteFrames = frames;
        Offset = new Vector2(0, textures[20].Texture.GetSize().Y / 2);
        Position = start;
        /*var ani = new AnimatedSprite2D()
        {
            Centered = false,
            SpriteFrames = frames,
            YSortEnabled = true,
            Autoplay = "default",
        };*/
        //var distance = start.DistanceTo(end);
        //_lengthSeconds = distance * 0.15f;
        //_lengthSeconds = distance * 0.0015f;
        _lengthSeconds = (float)millis / 1000;
        var diff = end - Position;
        Rotation = diff.Normalized().Angle();
        _velocity = diff / _lengthSeconds;
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
    
    public static Projectile Create(Vector2 start, Vector2 end, string sprite, int millis)
    {
        var textures = SpriteLoader.Load(sprite);
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://scene/projectile.tscn");
        var arrow = scene.Instantiate<Projectile>();
        arrow.Init(start, end, textures, millis);
        return arrow;
    }

    // public static Projectile Create(string id)
    // {
    //     var textures = SpriteLoader.Load("y2");
    // }
    
}