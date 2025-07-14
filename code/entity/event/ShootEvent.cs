using Godot;

namespace QnClient.code.entity.@event;

public readonly struct ShootEvent(long targetId, Vector2 start, string srpite, int flyMillis)
{
    public long TargetId { get; } = targetId;

    public Vector2 Start { get; } = start;
    
    public string Sprite { get; } = srpite;

    public int FlyMillis { get; } = flyMillis;
}