using System;
using Godot;
using QnClient.code.message;
using QnClient.code.util;

namespace QnClient.code.entity;

public abstract partial class AbstractEntity : Node2D, IEntity
{
    public event Action<IEntityEvent>? OnEntityEvent;
    
    public long Id { get; protected set; }
    
    public Vector2I Coordinate => Position.ToCoordinate();

    public abstract void HandleEntityMessage(IEntityMessage message);
    
    public void EmitEvent(IEntityEvent entityEvent)
    {
        OnEntityEvent?.Invoke(entityEvent);
    }
}