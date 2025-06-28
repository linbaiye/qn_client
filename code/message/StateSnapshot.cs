using QnClient.code.entity;
using QnClient.code.player;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class StateSnapshot<TS>(TS state, int elapsedMillis, MoveAction? action = null)
{
    public TS State { get; } = state;
    public int ElapsedMillis { get; } = elapsedMillis;

    public MoveAction? MoveAction { get; } = action;

    public override string ToString()
    {
        return $"{nameof(State)}: {State}, {nameof(ElapsedMillis)}: {ElapsedMillis}";
    }
    public static StateSnapshot<NpcState> OfMonster(InterpolationPacket packet)
    {
        return new StateSnapshot<NpcState>((NpcState)packet.State, packet.ElapsedMillis);
    }
    
    public static StateSnapshot<PlayerState> OfPlayer(InterpolationPacket packet)
    {
        return new StateSnapshot<PlayerState>((PlayerState)packet.State, packet.ElapsedMillis, packet.MoveAction != -1 ? (MoveAction)packet.MoveAction : null);
    }
}