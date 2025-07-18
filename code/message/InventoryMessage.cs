using System.Collections.Generic;
using QnClient.code.hud;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class InventoryMessage(List<InventoryItemMessage> items, bool force) : IHUDMessage
{
    public List<InventoryItemMessage> Items { get; } = items;
    

    public bool Forceful => force;

    public static InventoryMessage FromPacket(InventoryPacket packet)
    {
        List<InventoryItemMessage> items = new List<InventoryItemMessage>();
        foreach (var itemPacket in packet.Items)
        {
            items.Add(new InventoryItemMessage(itemPacket.Name, itemPacket.Icon, itemPacket.SlotId, itemPacket.Number, itemPacket.Color));
        }
        return new InventoryMessage(items, packet.Forceful);
    }

    public void ReplaceOrAdd(InventoryItemMessage message)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Slot == message.Slot)
            {
                Items[i] = message;
                return;
            }
        }
        Items.Add(message);
    }

    public void Accept(IHUDMessageHandler handler)
    {
        handler.UpdateInventoryView(this);
    }
}