using QnClient.code.entity;
using QnClient.code.player;
using QnClient.code.player.character;

namespace QnClient.code.message;

public readonly struct CreatureSayMessage(long id, string text) : ICharacterMessage, INpcMessage, IPlayerMessage
{
    public long Id { get; } = id;

    public string Text { get; } = text;
    
    public void Accept(INpcMessageHandler handler)
    {
    }

    public void Accept(IPlayerMessageHandler handler)
    {
    }

    public void Accpet(ICharacterMessageHandler handler)
    {
        handler.Say(this);
    }

    public static CreatureSayMessage Test(Character character, string message)
    {
        return new CreatureSayMessage(character.Id, message);
    }
}