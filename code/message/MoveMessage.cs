using System.Linq.Expressions;
using Godot;
using QnClient.code.entity;
using QnClient.code.player;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class MoveMessage(CreatureDirection direction, long id, Vector2I start, MoveAction? action = null) : AbstractEntityMessage(id),
    IPlayerMessage, INpcMessage
{
    public CreatureDirection Direction { get; } = direction;
    public MoveAction? Action { get; } = action;

    public Vector2I Start { get; } = start;

    public void Accept(IPlayerMessageHandler messageHandler)
    {
        messageHandler.Move(this);
    }

    public void Accept(INpcMessageHandler handler)
    {
        handler.Move(this);
    }

    public static MoveMessage FromPacket(NpcMovePacket packet)
    {
        return new MoveMessage((CreatureDirection)packet.Direction, packet.Id, new Vector2I(packet.X, packet.Y));
    }

    public static MoveMessage FromPacket(PlayerMovePacket packet)
    {
        return new MoveMessage((CreatureDirection)packet.MovePacket.Direction, packet.MovePacket.Id, new Vector2I(packet.MovePacket.X, packet.MovePacket.Y), (MoveAction)packet.MoveAction);
    }

}