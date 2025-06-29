using QnClient.code.entity;
using QnClient.code.player;
using QnClient.code.player.character;

namespace QnClient.code.message;

public readonly struct CreatureSayMessage(long id, string text) : INpcMessage, IPlayerMessage, ICharacterMessage
{
    public long Id { get; } = id;

    public string Text { get; } = text;
    
    public void Accept(INpcMessageHandler handler)
    {
        handler.Say(this);
    }

    public void Accept(IPlayerMessageHandler handler)
    {
        handler.Say(this);
    }

    public static CreatureSayMessage Test(Character character, string message)
    {
        return new CreatureSayMessage(character.Id, message);
    }

    public void Accept(ICharacterMessageHandler handler)
    {
        handler.Say(this);
    }
}