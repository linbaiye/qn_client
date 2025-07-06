using QnClient.code.hud;

namespace QnClient.code.message;

public record TextMessage(string text)  : IHUDMessage
{
    public void Accept(IHUDMessageHandler handler)
    {
        handler.DisplayText(text);
    }
}