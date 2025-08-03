using QnClient.code.entity;
using QnClient.code.hud;
using QnClient.code.player;
using QnClient.code.player.character;

namespace QnClient.code.message;

public readonly struct CreatureSayMessage(long id, string text, string name, bool cache) : INpcMessage, IPlayerMessage, ICharacterMessage, IHUDMessage
{
    public long Id { get; } = id;

    public string Text { get; } = text;

    private string Name { get; } = name;
    
    private bool Cache { get; } = cache;
    
    public void Accept(INpcMessageHandler handler)
    {
        handler.Say(this);
    }

    public void Accept(IPlayerMessageHandler handler)
    {
        handler.Say(this);
    }

    public void Accept(ICharacterMessageHandler handler)
    {
        handler.Say(this);
    }

    public void Accept(IHUDMessageHandler handler)
    {
        if (Cache)
            handler.CreatureSay(Name + "：" + Text);
    }
}