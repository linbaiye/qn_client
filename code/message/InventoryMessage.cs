using System.Collections.Generic;
using QnClient.code.hud;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class InventoryMessage(List<InventoryMessage.Item> items, bool force) : IHUDMessage
{
    public readonly struct Item(string name, int icon, int slot, long number, int color)
    {
        private string Name { get; } = name;
        public int Icon { get; } = icon;
        public int Slot { get; } = slot;
        private long Number { get; } = number;

        public int Color { get; } = color;

        public string ToolTip => Number != -1 ? Name + ": " + Number : Name;
    }

    public List<Item> Items { get; } = items;

    public bool Forceful => force;

    public static InventoryMessage FromPacket(InventoryPacket packet)
    {
        List<Item> items = new List<Item>();
        foreach (var itemPacket in packet.Items)
        {
            items.Add(new Item(itemPacket.Name, itemPacket.Icon, itemPacket.SlotId, itemPacket.Number, itemPacket.Color));
        }
        return new InventoryMessage(items, packet.Forceful);
    }

    public void Accept(IHUDMessageHandler handler)
    {
        handler.UpdateInventoryView(this);
    }
}