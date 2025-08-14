using System;
using Godot;
using QnClient.code.entity.@event;
using QnClient.code.message;

namespace QnClient.code.entity;

public partial class GuildStone : AbstractGroundItem
{
    public event Action<long>? AttackTriggered;
    
    public event Action<long>? Clicked;
    
    private GroundItemSnapshot _snapshot;
    
    public override void _Ready()
    {
        base._Ready();
        BodySprite.Clicked += () => Clicked?.Invoke(Id);
        BodySprite.AttackInvoked += () => AttackTriggered?.Invoke(Id);
        BodySprite.AttachShadowShader();
        _snapshot = null;
        EmitEvent(new EntityChangeCoordinateEvent(this));
        Visible = true;
    }

    public override bool HasMouseHover()
    {
        return BodySprite.HasMouseHover();
    }
    
    public static GuildStone Create(GroundItemSnapshot snapshot)
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://scene/ground_item.tscn");
        var itm = scene.Instantiate<GuildStone>();
        itm._snapshot = snapshot;
        return itm;
    }
}