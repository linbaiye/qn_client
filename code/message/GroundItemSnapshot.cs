using Godot;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class GroundItemSnapshot(long id, int color, string name, int number, Vector2I coordinate, int icon)
{
    public long Id { get; } = id;
    public string Name { get; } = name;
    public int Number { get; } = number;

    public int Color { get; } = color;

    public Vector2I Coordinate { get; } = coordinate;

    public int Icon => icon;

    public static GroundItemSnapshot FromPacket(ShowItemPacket p)
    {
        return new GroundItemSnapshot(p.Id, p.Color, p.Name, p.Number, new Vector2I(p.CoordinateX, p.CoordinateY), p.Icon);
    }

    public static GroundItemSnapshot Test()
    {
        return new GroundItemSnapshot(19999999L, 1, "皮", 15, new Vector2I(171, 235), 56);
        //return new GroundItemSnapshot(19999999L, 85, "男子黄龙弓服", 1, new Vector2I(171, 235), 29);
    }
}