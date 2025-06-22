namespace QnClient.code.message;

public interface IMessageHandler
{
    void Handle(JoinRealmMessage message);
}