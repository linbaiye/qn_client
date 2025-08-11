using QnClient.code.hud;

namespace QnClient.code.message;

public class DisconnectedMessage : IHUDMessage
{
    public static readonly DisconnectedMessage Instance = new();
    public void Accept(IHUDMessageHandler handler)
    {
        handler.DisplayBottomText("和服务器的连接已断开。", "yellow", "");
    }
}