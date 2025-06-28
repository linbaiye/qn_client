using QnClient.code.message;

namespace QnClient.code.entity;

public interface IEntityMessageHandler
{
    void Remove(RemoveEntityMessage message);

}