using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class RegisterAccountRequest(string name, string passwd) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            RegisterRequest = new RegisterRequestPacket()
            {
                Username = name,
                Password = passwd
            }
        };
    }
}