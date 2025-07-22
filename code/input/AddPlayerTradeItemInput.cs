using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class AddPlayerTradeItemInput(int slot, int number) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            AddPlayerTradeInput = new AddTradeItemInputPacket()
            {
                Slot = slot,
                Number = number
            }
        };
    }
}