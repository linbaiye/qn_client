using Godot;
using QnClient.code.entity;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public abstract class AbstractCreatureSnapshot(CreatureBaseInfoPacket packet)
{

    public string Name { get; } = packet.ViewName;

    public long Id { get; } = packet.Id;

    public Vector2I Coordinate { get;  } = new(packet.X, packet.Y);

    public CreatureDirection Direction { get;  } = (CreatureDirection)packet.Direction;

    public int ElapsedMillis { get;  } = packet.ElapsedMillis;
}