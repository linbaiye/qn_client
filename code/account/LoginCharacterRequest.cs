using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.account;

public class LoginCharacterRequest(string name) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            LoginCharacter = new LoginCharacterRequestPacket()
            {
                CharacterName = name,
            }
        };
    }
}