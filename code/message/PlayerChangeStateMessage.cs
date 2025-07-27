using Godot;
using QnClient.code.entity;
using QnClient.code.player;
using QnClient.code.player.character;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class PlayerChangeStateMessage(long id, PlayerState state, CreatureDirection direction, Vector2I coordinate) : AbstractEntityMessage(id), ICharacterMessage, IPlayerMessage
{
    public void Accept(ICharacterMessageHandler handler)
    {
        handler.ChangeState(coordinate, state, direction);
    }

    public static PlayerChangeStateMessage FromPacket(PlayerChangeStatePacket packet)
    {
        return new PlayerChangeStateMessage(packet.Id, (PlayerState)packet.State, (CreatureDirection)packet.Direction, new Vector2I(packet.X, packet.Y));
    }

    public void Accept(IPlayerMessageHandler handler)
    {
        handler.ChangeState(coordinate, state, direction);
    }
}