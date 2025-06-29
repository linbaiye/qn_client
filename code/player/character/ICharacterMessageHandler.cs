using QnClient.code.message;

namespace QnClient.code.player.character;

public interface ICharacterMessageHandler
{
    
    void Equip(PlayerEquipMessage message);

    void Say(CreatureSayMessage sayMessage);

}