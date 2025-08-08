using System.Collections.Generic;
using Google.Protobuf.Collections;
using QnClient.code.hud;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public readonly struct InventoryItemMessage(string name, int icon, int slot, long number = 0, int color = 0) : IHUDMessage
{
    public string Name { get; } = name;
    public int Icon { get; } = icon;
    public int Slot { get; } = slot;
    public long Number { get; } = number;
    public int Color { get; } = color;

    public bool Removed => string.IsNullOrEmpty(Name);
    public string Tip => Number != 1 ? Name + ": " + Number : Name;

    public static InventoryItemMessage FromPacket(InventoryItemPacket packet)
    {
        long number = packet.HasNumber && packet.Number > 0 ? packet.Number : 1;
        return new InventoryItemMessage(packet.Name, packet.Icon, packet.SlotId, number, packet.Color);
    }

    public static List<InventoryItemMessage> FromPacket(RepeatedField<InventoryItemPacket> packets)
    {
        List<InventoryItemMessage> items = new List<InventoryItemMessage>();
        foreach (var itemPacket in packets)
        {
            items.Add(FromPacket(itemPacket));
        }
        return items;
    }

    public void Accept(IHUDMessageHandler handler)
    {
        handler.UpdateInventorySlot(this);
    }
}