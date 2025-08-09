using QnClient.code.entity;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class NpcSnapshot  : AbstractCreatureSnapshot, INpcMessage, ISpriteMessage
{
	private NpcSnapshot(NpcSnapshotPacket packet)  : base(packet.BaseInfo)
    {
        Animate = packet.Animate;
        Shape = packet.Shape;
        NpcState = (NpcState)packet.State;
        DurationMillis = packet.Speed;
    }

    public int DurationMillis { get; }

    public void Accept(INpcMessageHandler handler)
    {
	    handler.Initialize(this);
    }
    
    public NpcState NpcState { get;private init; }
	   
    public string Animate { get; private init; }
	   
    public string Shape { get; }
    
    public static NpcSnapshot FromPacket(NpcSnapshotPacket packet)
    {
        return new NpcSnapshot(packet);
    }

    public string[] Sprites => [Shape];
}