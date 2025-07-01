using QnClient.code.entity;
using QnClient.code.message;

namespace QnClient.code.player.character;

public interface ICharacterMessageHandler
{
    
    void Equip(PlayerEquipMessage message);

    void Say(CreatureSayMessage message);
    
    void SyncActiveKungFuList(SyncActiveKungFuListMessage message);
    
    void ChangeState(PlayerState newState, CreatureDirection direction);

    void Unequip(EquipmentType type);

}