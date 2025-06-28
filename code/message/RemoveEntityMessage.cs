using QnClient.code.entity;
using QnClient.code.player;

namespace QnClient.code.message;

public class RemoveEntityMessage(long id) : AbstractEntityMessage(id), INpcMessage, IPlayerMessage
{
    public void Accept(INpcMessageHandler handler)
    {
        handler.Remove(this);
    }

    public void Accept(IPlayerMessageHandler handler)
    {
        handler.Remove(this);
    }
}