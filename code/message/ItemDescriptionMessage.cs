using QnClient.code.hud;
using QnClient.code.player;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class ItemDescriptionMessage(int type, int index, string text) : IHUDMessage
{
    private const int Inventory = 0;
    private const int KungFu = 1;
    private const int AttributeEquipment = 2;

    public static ItemDescriptionMessage FromPacket(ItemDescriptionPacket packet)
    {
        return new ItemDescriptionMessage(packet.Type, packet.Index, packet.Text);
    }

    public void Accept(IHUDMessageHandler handler)
    {
        if (type == Inventory)
            handler.ShowInventoryItemDescription(index, text);
        else if (type == KungFu)
            handler.ShowKungFuDescription(index, text);
        else if (type == AttributeEquipment)
            handler.ShowEquipmentDescription((EquipmentType)index, text);
    }
}