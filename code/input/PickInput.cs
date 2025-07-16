using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public struct PickInput(long id) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        throw new System.NotImplementedException();
    }
}