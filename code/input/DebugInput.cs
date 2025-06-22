using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class DebugInput : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            Debug = new DebugPacket()
            {
                Padding = 1
            }
        };
    }
}