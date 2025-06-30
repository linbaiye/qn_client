using QnClient.code.message;
using QnClient.code.ui.bottom;

namespace QnClient.code.player.character;

public interface ICharacterMessageHandler
{
    
    void Equip(PlayerEquipMessage message);

    void Say(CreatureSayMessage message);
    
    void SyncActiveKungFuList(SyncActiveKungFuListMessage message);

}