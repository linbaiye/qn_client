using Godot;
using QnClient.code.hud;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class StartDropItemMessage(int number, string name, int slot, Vector2I coordinate) : IHUDMessage
{
    public void Accept(IHUDMessageHandler handler)
    {
        handler.StartDropItem(name, number, slot, coordinate);
    }

    public static StartDropItemMessage FromPacket(StartDopItemPacket packet)
    {
        return new StartDropItemMessage(packet.MaxNumber, packet.Name, packet.Slot, new Vector2I(packet.X, packet.Y));
    }
}