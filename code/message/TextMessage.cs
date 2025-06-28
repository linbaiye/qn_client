using QnClient.code.ui;

namespace QnClient.code.message;

public record TextMessage(string text)  : IHUDMessage
{
    public void Accept(IHUDMessageHandler handler)
    {
        handler.DisplayText(text);
    }
}