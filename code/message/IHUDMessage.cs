using QnClient.code.ui;

namespace QnClient.code.message;

public interface IHUDMessage
{
    void Accept(IHUDMessageHandler handler);
}