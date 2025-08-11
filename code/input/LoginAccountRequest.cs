using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class LoginAccountRequest(string user, string passwd) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            LoginRequest = new LoginRequestPacket()
            {
                Username = user,
                Password = passwd
            }
        };
    }
}