using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class LoginInput : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            LoginPacket = new PlayerLoginPacket()
            {
                Token = "test",
                CharName = "test",
            }
        };
    }
}