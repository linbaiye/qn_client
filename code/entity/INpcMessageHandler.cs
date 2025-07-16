using QnClient.code.message;

namespace QnClient.code.entity;

public interface INpcMessageHandler : IEntityMessageHandler
{
    void Move(MoveMessage message);

    void SetPosition(SetPositionMessage message);

    void ChangeState(NpcChangeStateMessage message);

    void Initialize(NpcSnapshot snapshot);

    void Say(CreatureSayMessage message);
    
    void ShowLifeBar(int percent);
    
    void FireProjectile(long targetId, string sprite, int flyMillis);
}