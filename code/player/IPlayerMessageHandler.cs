using QnClient.code.entity;
using QnClient.code.message;

namespace QnClient.code.player;

public interface IPlayerMessageHandler : IEntityMessageHandler
{
    void SetPosition(SetPositionMessage message);
    
    void Move(MoveMessage message);

    void Equip(PlayerEquipMessage message);

    void Initialize(PlayerSnapshot snapshot);

    void Say(CreatureSayMessage message);

    void ChangeState(PlayerState newState, CreatureDirection direction);
    
    void Unequip(EquipmentType type);
    
    void Attack(AttackAction action, CreatureDirection direction, string effect);
    
    void ShowLifeBar(int percent);


}