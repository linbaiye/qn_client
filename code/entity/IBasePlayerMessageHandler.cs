using Godot;
using QnClient.code.message;
using QnClient.code.player;

namespace QnClient.code.entity;

/// <summary>
/// Handle messages that can be handled by both players and character.
/// </summary>
public interface IBasePlayerMessageHandler 
{
    void Equip(PlayerEquipMessage message);
    
    void Say(CreatureSayMessage message);

    void ChangeState(Vector2I coordinate, PlayerState newState, CreatureDirection direction);
    
    void Unequip(EquipmentType type);
    
    void Attack(AttackAction action, CreatureDirection direction, string effect);
    
    void ShowLifeBar(int percent);
    
    void FireProjectile(long targetId, string sprite, int flyMillis);

    void FollowRope(FollowRopeMessage message);
}