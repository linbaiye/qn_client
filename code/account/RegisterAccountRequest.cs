using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.account;

public class RegisterAccountRequest(string name, string passwd) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            RegisterAccount= new RegisterAccountRequestPacket()
            {
                Username = name,
                Password = passwd
            }
        };
    }
}