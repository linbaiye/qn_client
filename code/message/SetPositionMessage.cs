using Godot;
using QnClient.code.entity;
using QnClient.code.player;
using QnClient.code.player.character;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public readonly struct SetPositionMessage(long id, Vector2I coordinate, CreatureDirection direction, PlayerState state) : INpcMessage, IPlayerMessage, ICharacterMessage
{
    public long Id => id;
    public void Accept(ICharacterMessageHandler handler)
    {
        handler.SetPosition(Coordinate, state, direction);
    }

    public void Accept(IPlayerMessageHandler handler)
    {
        handler.SetPosition(this);
    }

    public void Accept(INpcMessageHandler handler)
    {
        handler.SetPosition(this);
    }

    public Vector2I Coordinate => coordinate;
    
    public CreatureDirection Direction => direction;

    public PlayerState State => state;

    public static SetPositionMessage FromPacket(PlayerSetPositionPacket packet)
    {
        return new SetPositionMessage(packet.Position.Id, new Vector2I(packet.Position.X, packet.Position.Y), (CreatureDirection)packet.Position.Direction, (PlayerState)packet.State);
    }
    
    // For npc.
    public static SetPositionMessage FromPacket(PositionPacket packet)
    {
        return new SetPositionMessage(packet.Id, new Vector2I(packet.X, packet.Y), (CreatureDirection)packet.Direction, PlayerState.Attack);
    }
}