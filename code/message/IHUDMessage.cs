using QnClient.code.hud;

namespace QnClient.code.message;

public interface IHUDMessage
{
    void Accept(IHUDMessageHandler handler);
}