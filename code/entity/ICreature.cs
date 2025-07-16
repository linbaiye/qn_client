using Godot;

namespace QnClient.code.entity;

public interface ICreature : IEntity
{
    Vector2 ProjectileAimPoint { get; }
    
    
}