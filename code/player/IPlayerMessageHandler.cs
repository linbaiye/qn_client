using QnClient.code.entity;
using QnClient.code.message;

namespace QnClient.code.player;

public interface IPlayerMessageHandler : IBasePlayerMessageHandler, IEntityMessageHandler
{
    void Move(MoveMessage message);
    
    void SetPosition(SetPositionMessage message);
    
    void Initialize(PlayerSnapshot snapshot);
}