using System;
using Godot;
using QnClient.code.entity.@event;
using QnClient.code.input;
using QnClient.code.message;
using QnClient.code.sprite;
using QnClient.code.util;

namespace QnClient.code.entity;

public partial class GroundItem : AbstractEntity
{
    private BodySprite _bodySprite;

    private Label _tip;

    public event Action<PickInput>? Picked;
    
    public override void _Ready()
    {
        _tip = GetNode<Label>("Tip");
        _bodySprite = GetNode<BodySprite>("Body");
        _bodySprite.MouseEntered += () => _tip.Visible = true;
        _bodySprite.MouseExited += () => _tip.Visible = false;
        _bodySprite.Clicked += OnPicked;
        _tip.Visible = false;
        Visible = false;
    }
    
    public void Init(GroundItemSnapshot snapshot)
    {
        var icons = ZipFileSpriteLoader.Instance.LoadOrderedItemIcons();
        var icon = icons[snapshot.Icon];
        var shaderMaterial = DyeShader.CreateShaderMaterial(snapshot.Color);
        Id = snapshot.Id;
        Position = snapshot.Coordinate.ToPosition();
        _bodySprite.Texture = icon;
        _bodySprite.Material = shaderMaterial;
        _bodySprite.Position = new Vector2(16, 12) - icon.GetSize() / 2;
        _bodySprite.MouseArea.Position = _bodySprite.Position;
        _bodySprite.MouseArea.Size = _bodySprite.Texture.GetSize();
        var tip = snapshot.Number > 1 ? snapshot.Name + ": " + snapshot.Number : snapshot.Name;
        _tip.Text = tip;
        var size = _tip.GetTextSize(tip);
        _tip.Position = new Vector2(16, 12) - size / 2;
        Visible = true;
    }

    private void OnPicked()
    {
          Picked?.Invoke(new PickInput(Id));
    }

    public static GroundItem Create()
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://scene/ground_item.tscn");
        return scene.Instantiate<GroundItem>();
    }
    
    public override void HandleEntityMessage(IEntityMessage message)
    {
        if (message is RemoveEntityMessage)
        {
            EmitEvent(new DeletedEvent(this));
            QueueFree();
        }
    }

    public void Remove()
    {
        
    }
}