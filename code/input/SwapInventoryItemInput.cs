using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public readonly struct SwapInventoryItemInput(int slot1, int slot2) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            SwapInventorySlotPacket = new SwapInventorySlotPacket()
            {
                Slot1 = slot1,
                Slot2 = slot2,
            }
        };
    }
}