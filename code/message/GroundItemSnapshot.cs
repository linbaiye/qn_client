using Godot;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class GroundItemSnapshot(long id, int color, string name, int number, Vector2I coordinate, int icon, bool groundStone = false, bool demo = false)
{
    public long Id { get; } = id;
    public string Name { get; } = name;
    public int Number { get; } = number;

    public int Color { get; } = color;

    public Vector2I Coordinate { get; } = coordinate;

    public int Icon { get; } = icon;

    public static GroundItemSnapshot FromPacket(ShowItemPacket p)
    {
        return new GroundItemSnapshot(p.Id, p.Color, p.Name, p.Number, new Vector2I(p.CoordinateX, p.CoordinateY), p.Icon, p.GuildStone, p.Demo);
    }
    
    public bool GroundStone { get;  } = groundStone;
    public bool Demo { get;  } = demo;


}