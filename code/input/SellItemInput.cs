using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class SellItemInput (long id, int slot, int number) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            SellItem = new SellItemInputPacket()
            {
                Id = id,
                Slot = slot,
                Number = number
            }
        };
    }
}