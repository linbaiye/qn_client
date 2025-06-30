using Godot;
using QnClient.code.entity;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class NpcSnapshot  : AbstractCreatureSnapshot, INpcMessage
{
	private NpcSnapshot(NpcSnapshotPacket packet)  : base(packet.BaseInfo)
    {
        Animate = packet.Animate;
        Shape = packet.Shape;
        NpcState = (NpcState)packet.State;
    }


    public void Accept(INpcMessageHandler handler)
    {
	    handler.Initialize(this);
    }
    
    public NpcState NpcState { get;private init; }
	   
    public string Animate { get; private init; }
	   
    public string Shape { get; private init; }
    
    public static NpcSnapshot FromPacket(NpcSnapshotPacket packet)
    {
        return new NpcSnapshot(packet);
    }
}