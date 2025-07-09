using QnClient.code.entity;
using QnClient.code.player;
using QnClient.code.player.character;

namespace QnClient.code.message;

public class EntityLifeBarMessage(long id, int lifePercent) : ICharacterMessage, IPlayerMessage, INpcMessage
{
    public long Id { get; } = id;
    
    public void Accept(INpcMessageHandler handler)
    {
        handler.ShowLifeBar(lifePercent);
    }

    public void Accept(IPlayerMessageHandler handler)
    {
        handler.ShowLifeBar(lifePercent);
    }

    public void Accept(ICharacterMessageHandler handler)
    {
        handler.ShowLifeBar(lifePercent);
    }
}