using QnClient.code.hud;

namespace QnClient.code.message;

public class GainExpMessage(string name) : IHUDMessage
{
    public void Accept(IHUDMessageHandler handler)
    {
        handler.BlinkText(name);
    }
}