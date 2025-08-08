using System.Collections.Generic;
using QnClient.code.hud;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class ShowBankMessage(long npcId, int capacity, int unlocked, List<InventoryItemMessage> itemMessages) : IHUDMessage
{
    public long NpcId => npcId;

    public int Capacity => capacity;

    public int Unlocked => unlocked;

    public List<InventoryItemMessage> ItemMessages => itemMessages;

    public static ShowBankMessage FromPacket(ShowBankWindowPacket packet)
    {
        List<InventoryItemMessage> itemMessages = InventoryItemMessage.FromPacket(packet.Items);
        return new ShowBankMessage(packet.BankerId, packet.Capacity, packet.Unlocked, itemMessages);
    }

    public void Accept(IHUDMessageHandler handler)
    {
        handler.ShowBank(this);
    }
}