using Source.Networking.Protobuf;

namespace QnClient.code.account;

public class CreateCharacterResponse(int code, string msg, string characterName)
{
    public int Code { get; } = code;
    public string Msg { get; } = msg;
    public string CharacterName { get; } = characterName;
    
    public static CreateCharacterResponse FromPacket(CreateCharacterResponsePacket packet)
    {
        return new CreateCharacterResponse(packet.Code, packet.Description, packet.CharacterName);
    }
}