namespace QnClient.code.message;

public abstract class AbstractEntityMessage(long id) : IEntityMessage
{
    public long Id => id;
}