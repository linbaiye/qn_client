using QnClient.code.entity;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class NpcChangeStateMessage(long id, NpcState state,CreatureDirection direction) : AbstractEntityMessage(id), INpcMessage
{
    public void Accept(INpcMessageHandler handler)
    {
        handler.ChangeState(this);
    }

    public NpcState State { get; } = state;
    public CreatureDirection Direction { get; } = direction;

    public override string ToString()
    {
        return $"{nameof(State)}: {State}, {nameof(Direction)}: {Direction}";
    }

    public static NpcChangeStateMessage FromPacket(ChangeStatePacket packet)
    {
        return new NpcChangeStateMessage(packet.Id, (NpcState)packet.State, (CreatureDirection)packet.Direction);
    }
}