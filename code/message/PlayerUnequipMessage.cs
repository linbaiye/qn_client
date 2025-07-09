using QnClient.code.hud;
using QnClient.code.player;
using QnClient.code.player.character;

namespace QnClient.code.message;

public struct PlayerUnequipMessage(long id, EquipmentType t) : IPlayerMessage, ICharacterMessage, IHUDMessage
{
    public long Id { get; } = id;
    
    private bool BelongToCharacter { get; set; } = false;
    
    public void Accept(ICharacterMessageHandler handler)
    {
        BelongToCharacter = true;
        handler.Unequip(t);
    }

    public void Accept(IPlayerMessageHandler handler)
    {
        handler.Unequip(t);
    }

    public void Accept(IHUDMessageHandler handler)
    {
        if (BelongToCharacter)
            handler.Unequip(t);
    }
}