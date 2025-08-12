using QnClient.code.hud;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class TextMessage(string text, int location, string color, string bgColor, int type = 0)  : IHUDMessage
{
    public const int Bottom = 0;
    private const int Left = 1;
    private const int LeftUp = 2;
    
    public enum TextType
    {
        Normal = 0,
        PrivateChat = 1,
    }

    public TextType Type { get; } = (TextType)type;

    public string Text { get; } = text;
    
    public string Color { get; } = color ;
    public string BgColor { get; } = bgColor;
    
    public void Accept(IHUDMessageHandler handler)
    {
        if (location == Left)
            handler.DisplayLeftText(Text);
        else if (location == LeftUp)
        {
            handler.DisplayLeftUpText(Text);
            handler.DisplayBottomText(this);
        }
        else if (location == Bottom)
            handler.DisplayBottomText(this);
    }

    public static TextMessage FromPacket(TextMessagePacket packet)
    {
        return new TextMessage(packet.Text, packet.Location, packet.Color, packet.BgColor, packet.Type);
    }
}