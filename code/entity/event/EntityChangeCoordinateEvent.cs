namespace QnClient.code.entity.@event;

public class EntityChangeCoordinateEvent(IEntity entity) : IEntityEvent
{
    public IEntity Source => entity;

    public static EntityChangeCoordinateEvent Of(IEntity entity)
    {
        return new EntityChangeCoordinateEvent(entity);
    }
}