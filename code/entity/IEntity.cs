using Godot;
using QnClient.code.message;

namespace QnClient.code.entity;

public interface IEntity
{
    long Id { get; }
        
    Vector2I Coordinate { get; }

    void HandleEntityMessage(IEntityMessage message);

    bool IsCoveringPosition(Vector2 position);

}