using Godot;
using QnClient.code.entity;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class NpcSnapshot  : AbstractCreatureSnapshot, INpcMessage
{
	private NpcSnapshot(string name, StateSnapshot<NpcState> stateSnapshot, long id, string animate,
        string shape, CreatureDirection direction, Vector2I coordinate) 
    {
		  
        Name = name;
        StateSnapshot = stateSnapshot;
        Id = id;
        Animate = animate;
        Shape = shape;
        Direction = direction;
        Coordinate = coordinate;
    }


    public void Accept(INpcMessageHandler handler)
    {
	    handler.Initialize(this);
    }


    public override string ToString()
    {
        return $"{nameof(Name)}: {Name}, {nameof(Coordinate)}: {Coordinate}, {nameof(Direction)}: {Direction}, {nameof(StateSnapshot.ElapsedMillis)}: {StateSnapshot.ElapsedMillis},{nameof(StateSnapshot.State)}: {StateSnapshot.State}";
    }
    public StateSnapshot<NpcState> StateSnapshot { get; }
	   
    public string Animate { get; private init; }
	   
    public string Shape { get; private init; }
    
    public static NpcSnapshot FromPacket(CreatureInterpolationPacket packet)
    {
        return new NpcSnapshot(packet.Name, StateSnapshot<NpcState>.OfMonster(packet.Interpolation), packet.Id, packet.Animate, packet.Shape,
	        (CreatureDirection)packet.Interpolation.Direction, new Vector2I(packet.Interpolation.X, packet.Interpolation.Y));
    }
}