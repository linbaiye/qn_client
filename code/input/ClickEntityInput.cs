using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public readonly struct ClickEntityInput(long id) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            ClickPacket = new ClickPacket()
            {
                Id = id,
            }
        };
    }
}