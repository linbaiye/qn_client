using QnClient.code.hud;

namespace QnClient.code.message;

public record TextMessage(string text, int location)  : IHUDMessage
{
    private const int Left = 1;
    private const int LeftUp = 2;
    public void Accept(IHUDMessageHandler handler)
    {
        if (location == Left)
            handler.DisplayLeftText(text);
        else if (location == LeftUp)
        {
            handler.DisplayLeftUpText(text);
            handler.DisplayBottomText(text);
        }
        else
            handler.DisplayBottomText(text);
    }
}