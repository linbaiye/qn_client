using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.account;

public class CreateCharacterRequest(string name, bool male) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            CreateCharacter = new CreateCharacterRequestPacket()
            {
                CharacterName = name,
                Male = male
            }
        };
    }
}