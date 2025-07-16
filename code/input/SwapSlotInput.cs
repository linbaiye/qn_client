using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public readonly struct SwapSlotInput(ClientPacket clientPacket) : I2ServerMessage
{
    
    public ClientPacket ToPacket()
    {
        return clientPacket;
    }

    public static SwapSlotInput KungFu(int page, int slot1, int slot2)
    {
        ClientPacket packet = new ClientPacket() 
        {
            SwapKungFuSlotPacket = new SwapKungFuSlotPacket()
            {
                Page = page,
                Slot1 = slot1,
                Slot2 = slot2,
            }
        };
        return new SwapSlotInput(packet);
    }

    public static SwapSlotInput Inventory(int slot1, int slot2)
    {
        ClientPacket packet = new ClientPacket() 
        {
            SwapInventorySlotPacket= new SwapInventorySlotPacket()
            {
                Slot1 = slot1,
                Slot2 = slot2,
            }
        };
        return new SwapSlotInput(packet);
    }
}