using QnClient.code.player;
using QnClient.code.player.character;

namespace QnClient.code.message;

public readonly struct PlayerUnequipMessage(long id, EquipmentType t) : IPlayerMessage, ICharacterMessage
{
    public long Id { get; } = id;
    
    public void Accept(ICharacterMessageHandler handler)
    {
        handler.Unequip(t);
    }

    public void Accept(IPlayerMessageHandler handler)
    {
        handler.Unequip(t);
    }
}