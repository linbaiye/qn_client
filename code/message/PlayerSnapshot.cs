using QnClient.code.player;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class PlayerSnapshot : AbstractCreatureSnapshot, IPlayerMessage
{
    private PlayerSnapshot(PlayerSnapshotPacket packet)  : base(packet.BaseInfo)
    {
        Male = packet.Male;
        PlayerState = (PlayerState)packet.State;
        MoveAction = packet.MoveAction != -1 ? (MoveAction)packet.MoveAction : MoveAction.None;
    }
    public bool Male { get; private init; }
    
    public PlayerState PlayerState { get; private init; }
    
    public MoveAction MoveAction { get; private init; }

    public static PlayerSnapshot FromPacket(PlayerSnapshotPacket packet)
    {
        return new PlayerSnapshot(packet);
    }

    public void Accept(IPlayerMessageHandler handler)
    {
        handler.Initialize(this);
    }
}