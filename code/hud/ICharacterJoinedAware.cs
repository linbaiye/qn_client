using QnClient.code.message;

namespace QnClient.code.hud;

public interface ICharacterJoinedAware
{
    void OnCharacterJoined(JoinRealmMessage message);
}