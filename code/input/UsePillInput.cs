using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class UsePillInput(string name) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            UsePill = new UsePillInputPacket()
            {
                Name = name,
            }
        };
    }
}