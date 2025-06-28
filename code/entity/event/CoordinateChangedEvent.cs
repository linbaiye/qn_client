namespace QnClient.code.entity.@event;

public class CoordinateChangedEvent(IEntity entity) : IEntityEvent
{
    public IEntity Source => entity;

    public static CoordinateChangedEvent Of(IEntity entity)
    {
        return new CoordinateChangedEvent(entity);
    }
}