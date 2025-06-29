namespace QnClient.code.entity.@event;

public class EntityCoordinateEvent(IEntity entity) : IEntityEvent
{
    public IEntity Source => entity;

    public static EntityCoordinateEvent Of(IEntity entity)
    {
        return new EntityCoordinateEvent(entity);
    }
}