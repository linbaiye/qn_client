using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public readonly struct PickInput(long id) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            PickInput = new PickInputPacket()
            {
                Id = id,
            }
        };
    }
}