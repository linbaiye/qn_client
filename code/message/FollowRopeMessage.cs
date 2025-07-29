using Godot;
using QnClient.code.entity;
using QnClient.code.player;
using QnClient.code.player.character;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class FollowRopeMessage(long id, Vector2I coordinate, CreatureDirection direction, int durationMillis): IPlayerMessage, ICharacterMessage
{
    public long Id { get; } = id;
    
    public void Accept(ICharacterMessageHandler handler)
    {
        handler.FollowRope(this);
    }

    public void Accept(IPlayerMessageHandler handler)
    {
        handler.FollowRope(this);
    }

    public Vector2I Coordinate { get; } = coordinate;
    
    public CreatureDirection Direction { get; } = direction;
    
    public int DurationMillis { get; } = durationMillis;

    public static FollowRopeMessage FromPacket(FollowRopePacket packet)
    {
        return new FollowRopeMessage(packet.Id, new Vector2I(packet.X, packet.Y), (CreatureDirection)packet.Direction, packet.DurationMillis);
    }
}