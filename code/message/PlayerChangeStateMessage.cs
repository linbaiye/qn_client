using QnClient.code.entity;
using QnClient.code.player;
using QnClient.code.player.character;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class PlayerChangeStateMessage(long id, PlayerState state, CreatureDirection direction) : AbstractEntityMessage(id), ICharacterMessage
{
    public void Accept(ICharacterMessageHandler handler)
    {
        handler.ChangeState(state, direction);
    }

    public static PlayerChangeStateMessage FromPacket(PlayerChangeStatePacket packet)
    {
        return new PlayerChangeStateMessage(packet.Id, (PlayerState)packet.State, (CreatureDirection)packet.Direction);
    }
}