using QnClient.code.player.character;

namespace QnClient.code.message;

public interface ICharacterMessage: IEntityMessage
{
    void Accept(ICharacterMessageHandler handler);
}