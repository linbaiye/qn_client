using QnClient.code.entity;
using QnClient.code.player;
using QnClient.code.player.character;

namespace QnClient.code.message;

public class FlowRopeMessage : IPlayerMessage, ICharacterMessage
{
    public long Id { get; }
    public void Accept(ICharacterMessageHandler handler)
    {
        handler.FollowRope(CreatureDirection.Up);
    }

    public void Accept(IPlayerMessageHandler handler)
    {
        throw new System.NotImplementedException();
    }
}