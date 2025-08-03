using QnClient.code.hud;

namespace QnClient.code.message;

public record KungFuGainExpMessage(int l, string n, bool attack) : IHUDMessage
{
    public void Accept(IHUDMessageHandler handler)
    {
        handler.KungFuGainExp(n, l, attack);
    }
}