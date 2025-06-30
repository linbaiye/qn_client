using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public readonly struct ClickInventoryInput(int slot, int clickType) : I2ServerMessage
{
    
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            ClickInventorySlotInput = new ClickInventorySlotInputPacket()
            {
                Slot = slot,
                ClickType = clickType,
            }
        };
    }
    
    public static ClickInventoryInput LeftClick(int slot)
    {
        return new ClickInventoryInput(slot, 1);
    }
    
    public static ClickInventoryInput LeftDoubleClick(int slot)
    {
        return new ClickInventoryInput(slot, 2);
    }
    
    public static ClickInventoryInput RightClick(int slot)
    {
        return new ClickInventoryInput(slot, 3);
    }
}