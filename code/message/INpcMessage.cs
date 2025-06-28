using QnClient.code.entity;

namespace QnClient.code.message;

public interface INpcMessage : IEntityMessage
{
    void Accept(INpcMessageHandler handler);

}