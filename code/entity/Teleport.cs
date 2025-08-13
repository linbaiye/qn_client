using Godot;
using QnClient.code.entity.@event;
using QnClient.code.message;
using QnClient.code.sprite;
using QnClient.code.util;

namespace QnClient.code.entity;

public partial class Teleport : AbstractEntity
{
    private BodySprite _bodySprite;

    private Label _tip;
    
    public override void _Ready()
    {
        _tip = GetNode<Label>("Tip");
        _bodySprite = GetNode<BodySprite>("Body");
        ZIndex = 1;
    }

    public void Init(long id, string viewName, Vector2I coordinate, int icon)
    {
        Id = id;
        Position = coordinate.ToPosition();
        var icons = ZipFileSpriteLoader.Instance.LoadOrderedItemIcons();
        _bodySprite.Texture = icons[icon];
        var iconSize = _bodySprite.Texture.GetSize();
        _bodySprite.MouseArea.Size = _bodySprite.Texture.GetSize();
        _tip.Text = viewName;
        var size = _tip.GetTextSize(viewName);
        _tip.Position =  iconSize / 2 - (size / 2);
        Visible = true;
    }
    
    public override void HandleEntityMessage(IEntityMessage message)
    {
        if (message is RemoveEntityMessage)
        {
            EmitEvent(new DeletedEvent(this));
            QueueFree();
        }
    }

    public override bool HasMouseHover()
    {
        return false;
    }

    public static Teleport Create()
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://scene/teleport.tscn");
        return scene.Instantiate<Teleport>();
    }
}