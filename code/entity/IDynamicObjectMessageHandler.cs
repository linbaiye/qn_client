namespace QnClient.code.entity;

public interface IDynamicObjectMessageHandler : IEntityMessageHandler
{
    void ShowLifeBar(int percent);

    void Shift(int id, int id2, bool liftCoordinates);
}