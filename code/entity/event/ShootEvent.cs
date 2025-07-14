using Godot;

namespace QnClient.code.entity.@event;

public struct ShootEvent(Vector2 target, Vector2 start)
{
    public Vector2 Target { get; } = target;

    public Vector2 Start { get; } = start;
}