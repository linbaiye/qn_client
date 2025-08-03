using QnClient.code.hud;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class TextMessage(string text, int location, string color, string bgColor)  : IHUDMessage
{
    private const int Bottom = 0;
    private const int Left = 1;
    private const int LeftUp = 2;
    public void Accept(IHUDMessageHandler handler)
    {
        if (location == Left)
            handler.DisplayLeftText(text);
        else if (location == LeftUp)
        {
            handler.DisplayLeftUpText(text);
            handler.DisplayBottomText(text, color, bgColor);
        }
        else if (location == Bottom)
            handler.DisplayBottomText(text, color, bgColor);
    }

    public static TextMessage FromPacket(TextMessagePacket packet)
    {
        return new TextMessage(packet.Text, packet.Location, packet.Color, packet.BgColor);
    }
}