namespace QnClient.code.entity.@event;

public class LiftCoordinatesEvent(DynamicObject dynamicObject) : IEntityEvent
{
    public IEntity Source { get; } = dynamicObject;
}