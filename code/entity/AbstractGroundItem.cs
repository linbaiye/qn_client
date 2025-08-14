using System;
using Godot;
using NLog;
using QnClient.code.entity.@event;
using QnClient.code.message;
using QnClient.code.sprite;
using QnClient.code.util;

namespace QnClient.code.entity;

public abstract partial class AbstractGroundItem : AbstractEntity
{
    
    private BodySprite _bodySprite;

    private Label _tip;
    
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    public override void _Ready()
    {
        _tip = GetNode<Label>("Tip");
        _bodySprite = GetNode<BodySprite>("Body");
        _bodySprite.MouseEntered += () => _tip.Visible = true;
        _bodySprite.MouseExited += () => _tip.Visible = false;
        ZIndex = 2;
        _tip.Visible = false;
        Visible = false;
    }

    protected BodySprite BodySprite => _bodySprite;
    
    public String ItemName { get; private set; }
    
    protected void Initialize(GroundItemSnapshot snapshot)
    {
        var icons = ZipFileSpriteLoader.Instance.LoadOrderedItemIcons();
        var icon = icons[snapshot.Icon];
        Id = snapshot.Id;
        Position = snapshot.Coordinate.ToPosition();
        _bodySprite.Texture = icon;

        var iconSize = icon.GetSize();
        _bodySprite.Position = new Vector2(16, 12) - iconSize / 2;
        _bodySprite.MouseArea.Size = _bodySprite.Texture.GetSize();
        var tip = snapshot.Number > 1 ? snapshot.Name + ": " + snapshot.Number : snapshot.Name;
        _tip.Text = tip;
        var size = _tip.GetTextSize(tip);
        _tip.Position = new Vector2(16, 12) - size / 2;
        ItemName = snapshot.Name;
    }

    public override void HandleEntityMessage(IEntityMessage message)
    {
        if (message is RemoveEntityMessage)
        {
            EmitEvent(new DeletedEvent(this));
            QueueFree();
        }
    }
}