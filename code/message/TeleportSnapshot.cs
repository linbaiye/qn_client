using Godot;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class TeleportSnapshot(long id, string name, Vector2I coordinate, int icon)
{

    public long Id { get; } = id;

    public string Name { get; } = name;
    
    public Vector2I Coordinate { get; } = coordinate;
    
    public int Icon { get; } = icon;

    public static TeleportSnapshot FromPacket(ShowTeleportPacket packet)
    {
        return new TeleportSnapshot(packet.Id, packet.Name, new Vector2I(packet.CoordinateX, packet.CoordinateY),
            packet.Shape);
    }
    
}