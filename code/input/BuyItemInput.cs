using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class BuyItemInput(long id, string name, int number) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            BuyItem = new BuyItemInputPacket()
            {
                Id = id,
                Name = name,
                Number = number
            }
        };
    }
}

