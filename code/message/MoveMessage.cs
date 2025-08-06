using Godot;
using QnClient.code.entity;
using QnClient.code.player;
using QnClient.code.player.character;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class MoveMessage(CreatureDirection direction, long id, Vector2I start, MoveAction? action = null,  int durationMillis = 0, int startMillis = 0) : AbstractEntityMessage(id),
    IPlayerMessage, INpcMessage, ICharacterMessage
{
    public CreatureDirection Direction { get; } = direction;
    public MoveAction? Action { get; } = action;

    public Vector2I Start { get; } = start;

    public int StartMillis { get; } = startMillis;

    public void Accept(IPlayerMessageHandler messageHandler)
    {
        messageHandler.Move(this);
    }

    public void Accept(INpcMessageHandler handler)
    {
        handler.Move(this);
    }

    public int DurationMillis { get; } = durationMillis;

    public static MoveMessage FromPacket(NpcMovePacket packet)
    {
        return new MoveMessage((CreatureDirection)packet.Direction, packet.Id, new Vector2I(packet.X, packet.Y), null, packet.SpeedMillis);
    }

    public static MoveMessage FromPacket(PlayerMovePacket packet)
    {
        return new MoveMessage((CreatureDirection)packet.MovePacket.Direction, packet.MovePacket.Id, new Vector2I(packet.MovePacket.X, packet.MovePacket.Y), (MoveAction)packet.MoveAction, 0, packet.StartMillis);
    }

    public void Accept(ICharacterMessageHandler handler)
    {
        handler.RestoreMove(this);
    }
}