namespace QnClient.code.entity.@event;

public class DeletedEvent(IEntity entity) : IEntityEvent
{
    public IEntity Source { get; } = entity;
}