using QnClient.code.hud;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class UpdateTradeWindowSlotMessage(bool self, InventoryItemMessage item) : IHUDMessage
{
    public void Accept(IHUDMessageHandler handler)
    {
        handler.UpdateTradeWindowSlot(self, item);
    }

    public static UpdateTradeWindowSlotMessage FromPacket(UpdateTradeWindowSlotPacket packet)
    {
        var inventoryItemMessage = InventoryItemMessage.FromPacket(packet.Item);
        return new UpdateTradeWindowSlotMessage(packet.Self, inventoryItemMessage);
    }
}