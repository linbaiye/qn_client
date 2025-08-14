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
        Initialize(_snapshot);
        BodySprite.Clicked += () => Clicked?.Invoke(Id);
        BodySprite.AttackInvoked += () => AttackTriggered?.Invoke(Id);
        BodySprite.AttachShadowShader();
        if (_snapshot.Demo)
            Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0.6f);
        else
            EmitEvent(new EntityChangeCoordinateEvent(this));
        _snapshot = null;
        Visible = true;
    }

    public override bool HasMouseHover()
    {
        return BodySprite.HasMouseHover();
    }
    
    public static GuildStone Create(GroundItemSnapshot snapshot)
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://scene/guild_stone.tscn");
        var itm = scene.Instantiate<GuildStone>();
        itm._snapshot = snapshot;
        return itm;
    }
}