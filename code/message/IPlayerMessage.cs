using QnClient.code.player;

namespace QnClient.code.message;

public interface IPlayerMessage : IEntityMessage
{
    void Accept(IPlayerMessageHandler handler);

}