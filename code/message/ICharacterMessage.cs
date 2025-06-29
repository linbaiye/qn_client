using QnClient.code.player.character;

namespace QnClient.code.message;

public interface ICharacterMessage
{
    void Accpet(ICharacterMessageHandler handler);
}