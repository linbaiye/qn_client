using QnClient.code.hud;

namespace QnClient.code.message;

public class CloseTradeWindowMessage(int which) : IHUDMessage
{
    private const int PlayerTrade = 1;
    private const int Bank = 2;
    public void Accept(IHUDMessageHandler handler)
    {
        handler.ClosePlayerTradeWindow();
    }
}