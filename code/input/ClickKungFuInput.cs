using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public readonly struct ClickKungFuInput(int page, int slot, int clickType) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            ClickKungFuInput = new ClickKungFuInputPacket()
            {
                Page = page,
                Slot = slot,
                ClickType = clickType,
            }
        };
    }

    public static ClickKungFuInput LeftClick(int page, int slot)
    {
        return new ClickKungFuInput(page, slot, 1);
    }
    
    public static ClickKungFuInput LeftDoubleClick(int page, int slot)
    {
        return new ClickKungFuInput(page, slot, 2);
    }
    
    public static ClickKungFuInput RightClick(int page, int slot)
    {
        return new ClickKungFuInput(page, slot, 3);
    }
}