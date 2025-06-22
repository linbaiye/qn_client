namespace QnClient.code.message;

public interface IMessage
{
   void Accept(IMessageHandler messageHandler); 
}