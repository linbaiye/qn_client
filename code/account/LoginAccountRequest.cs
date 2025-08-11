using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.account;

public class LoginAccountRequest(string user, string passwd) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            LoginAccount= new LoginAccountRequestPacket()
            {
                Username = user,
                Password = passwd
            }
        };
    }
}