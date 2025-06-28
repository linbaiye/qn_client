using Godot;
using QnClient.code.entity;
using QnClient.code.player;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class PlayerSnapshot : AbstractCreatureSnapshot, IPlayerMessage
{
    private PlayerSnapshot(string name, StateSnapshot<PlayerState> stateSnapshot, long id, CreatureDirection direction, Vector2I coordinate, bool male) 
    {
		  
        Name = name;
        StateSnapshot = stateSnapshot;
        Id = id;
        Direction = direction;
        Coordinate = coordinate;
        Male = male;
    }
    
    public StateSnapshot<PlayerState> StateSnapshot { get; private init; }
    public bool Male { get; private init; }

    public static PlayerSnapshot FromPacket(PlayerInterpolationPacket packet)
    {
        var state = StateSnapshot<PlayerState>.OfPlayer(packet.Interpolation);
        return new PlayerSnapshot(packet.Info.Name, state, packet.Info.Id,
            (CreatureDirection)packet.Interpolation.Direction,
            new Vector2I(packet.Interpolation.X, packet.Interpolation.Y),
            packet.Info.Male);
    }

    public void Accept(IPlayerMessageHandler handler)
    {
        handler.Initialize(this);
    }
}