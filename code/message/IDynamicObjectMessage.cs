using QnClient.code.entity;

namespace QnClient.code.message;

public interface IDynamicObjectMessage : IEntityMessage
{
   void Accept(IDynamicObjectMessageHandler handler);
}