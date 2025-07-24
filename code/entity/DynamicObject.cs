using System.Collections.Generic;
using Godot;
using QnClient.code.entity.@event;
using QnClient.code.message;
using QnClient.code.util;

namespace QnClient.code.entity;

public partial class DynamicObject : AbstractEntity
{

    private BodySprite _bodySprite;

    private DynamicObjectAnimationPlayer _animationPlayer;

    private readonly ISet<Vector2I> _coordinates = new HashSet<Vector2I>();
    
    public override void _Ready()
    {
        base._Ready();
        _animationPlayer = GetNode<DynamicObjectAnimationPlayer>("AnimationPlayer");
    }

    public override void HandleEntityMessage(IEntityMessage message)
    {
    }

    public void Initialize(DynamicObjectSnapshot snapshot)
    {
        Position = snapshot.Coordinate.ToPosition();
        _animationPlayer.Initialize(snapshot.Shape, snapshot.Animations);
        foreach (var c in snapshot.Coordinates)
        {
            _coordinates.Add(c);
        }
        _coordinates.Add(snapshot.Coordinate);
        EmitEvent(new EntityChangeCoordinateEvent(this));
        _animationPlayer.PlayId(snapshot.AnimateId);
    }

    public void Play(InputEvent @event)
    {
        if (@event is InputEventKey key)
        {
            switch (key.Keycode)
            {
                case Key.Key1:
                    _animationPlayer.PlayId(1);
                    break;
                case Key.Key2:
                    _animationPlayer.PlayId(2);
                    break;
                case Key.Key3:
                    _animationPlayer.PlayId(3);
                    break;
            }
        }
    }

    public override bool IsCoveringPosition(Vector2 position)
    {
        var start = Position + _bodySprite.MouseArea.Position;
        var end = start + _bodySprite.MouseArea.GetSize();
        return start.X <= position.X && end.X >= position.X &&
               start.Y <= position.Y && end.Y >= position.Y;
    }

    public IEnumerable<Vector2I> Coordinates => _coordinates;
    
    public static DynamicObject Create()
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://scene/dynamic_object.tscn");
        return scene.Instantiate<DynamicObject>();
    }
}