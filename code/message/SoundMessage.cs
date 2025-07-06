using QnClient.code.hud;

namespace QnClient.code.message;

public class SoundMessage(string entityName, string sound) : IHUDMessage
{
    public void Accept(IHUDMessageHandler handler)
    {
        handler.PlaySound(entityName, sound);
    }
}