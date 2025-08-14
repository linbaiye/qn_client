using System;
using Godot;
using NLog;
using QnClient.code.input;
using QnClient.code.message;

namespace QnClient.code.entity;

public partial class GroundItem : AbstractGroundItem
{
    private BodySprite _bodySprite;

    private Label _tip;
    public event Action<PickInput>? Picked;
    
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private GroundItemSnapshot _snapshot;
    
    public override void _Ready()
    {
        base._Ready();
        BodySprite.Clicked += OnPicked;
        BodySprite.AttackInvoked += OnPicked;
        Initialize(_snapshot);
        var shaderMaterial = DyeShader.CreateShaderMaterial(_snapshot.Color);
        BodySprite.Material = shaderMaterial;
        Visible = true;
        _snapshot = null;
    }
    
    private void OnPicked()
    {
          Picked?.Invoke(new PickInput(Id));
    }

    public override bool HasMouseHover()
    {
        return false;
    }
    
    public static GroundItem Create(GroundItemSnapshot snapshot)
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://scene/ground_item.tscn");
        var itm = scene.Instantiate<GroundItem>();
        itm._snapshot = snapshot;
        return itm;
    }
}