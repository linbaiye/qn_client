using Source.Networking.Protobuf;

namespace QnClient.code.network.toserver
{
    public interface I2ServerMessage
    {
        ClientPacket ToPacket();
    }
}