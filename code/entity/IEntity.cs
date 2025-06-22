using Godot;

namespace QnClient.code.entity;

public interface IEntity
{
    string EntityName { get; }

    long Id { get; }
        
    Vector2I Coordinate { get; }
    
}