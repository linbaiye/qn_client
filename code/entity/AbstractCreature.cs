using System;
using Godot;
using QnClient.code.entity.@event;
using QnClient.code.message;
using QnClient.code.ui;
using QnClient.code.util;

namespace QnClient.code.entity;

public abstract partial class AbstractCreature : Node2D, IEntity, IEntityMessageHandler
{
    public event Action<IEntityEvent>? OnEntityEvent;

    private string _name;

    public string EntityName => _name;

    private TextBubble _textBubble;
    
    private BodySprite _bodySprite;

    public override void _Ready()
    {
        _textBubble = GetNode<TextBubble>("TextBubble");
        _bodySprite = GetNode<BodySprite>("Body");
    }
    
    private void SetViewName(string name)
    {
        _name = name;
        var label = GetNode<Label>("Name");
        label.Text = name;
        label.Visible = false;
        label.Resized += () =>
        {
            label.Position += VectorUtil.DefaultTextureOffset;
        };
    }

    public long Id { get; private set; }
    public Vector2I Coordinate => Position.ToCoordinate();
    
    public abstract void HandleEntityMessage(IEntityMessage message);

    public void EmitEvent(IEntityEvent entityEvent)
    {
        OnEntityEvent?.Invoke(entityEvent);
    }

    protected void Initialize(long id, Vector2I coordinate, string name)
    {
        Id = id;
        var bodySprite = GetNode<BodySprite>("Body");
        bodySprite.MouseEntered += () => GetNode<Label>("Name").Visible = true;
        bodySprite.MouseExited += () => GetNode<Label>("Name").Visible = false;
        bodySprite.AttackInvoked += () => GD.Print("Attack " + Id);
        bodySprite.Clicked += () => GD.Print("Clicked " + Id);
        Position = coordinate.ToPosition();
        SetViewName(name);
    }

    protected void Initialize(AbstractCreatureSnapshot snapshot)
    {
        Initialize(snapshot.Id, snapshot.Coordinate, snapshot.Name);
    }

    public void Remove(RemoveEntityMessage message)
    {
        EmitEvent(new DeletedEvent(this));
        QueueFree();
    }

    public void Say(CreatureSayMessage sayMessage)
    {
        _textBubble.Display(sayMessage.Text, _bodySprite.Texture.GetSize());
    }
}