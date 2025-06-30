using System;
using Godot;
using QnClient.code.entity.@event;
using QnClient.code.message;
using QnClient.code.ui;
using QnClient.code.util;

namespace QnClient.code.entity;

public abstract partial class AbstractEntity : Node2D, IEntity, IEntityMessageHandler
{
    public event Action<IEntityEvent>? OnEntityEvent;

    private string _name;

    public string EntityName => _name;
    
    private void SetViewName(string name)
    {
        _name = name;
        var label = GetNode<Label>("Label");
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

    protected void Initialize(AbstractCreatureSnapshot snapshot)
    {
        Id = snapshot.Id;
        var bodySprite = GetNode<BodySprite>("Body");
        bodySprite.MouseEntered += () => GetNode<Label>("Label").Visible = true;
        bodySprite.MouseExited += () => GetNode<Label>("Label").Visible = false;
        bodySprite.AttackInvoked += () => GD.Print("Attack " + Id);
        bodySprite.Clicked += () => GD.Print("Clicked " + Id);
        Position = snapshot.Coordinate.ToPosition();
        SetViewName(snapshot.Name);
    }

    public void Remove(RemoveEntityMessage message)
    {
        EmitEvent(new DeletedEvent(this));
        QueueFree();
    }
}