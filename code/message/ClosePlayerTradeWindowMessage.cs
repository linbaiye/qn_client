using QnClient.code.hud;

namespace QnClient.code.message;

public class ClosePlayerTradeWindowMessage : IHUDMessage
{
    public static readonly ClosePlayerTradeWindowMessage Instance = new();
    public void Accept(IHUDMessageHandler handler)
    {
        handler.CloseTradeWindow();
    }
}