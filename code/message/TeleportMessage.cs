using Godot;
using QnClient.code.hud;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class TeleportMessage(string file, string resource, string bgm, Vector2I coor, string title) : IHUDMessage
{

    public string MapFile { get; } = file;

    public string ResourceName { get; } = resource;

    public string Bgm { get; } = bgm;

    public Vector2I Coordinate { get; } = coor;

    public string MapTitle { get; } = title;

    public static TeleportMessage FromPacket(TeleportPacket packet)
    {
        var map = packet.Map.Replace(".map", "");
        var resource = packet.Resource.Replace(".zip", "");
        return new TeleportMessage(map, resource, packet.Bgm, new Vector2I(packet.X, packet.Y), packet.Title);
    }

    public void Accept(IHUDMessageHandler handler)
    {
        handler.OnCharacterTeleported(this);
    }
}