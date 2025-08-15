using QnClient.code.hud;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class ApplyKungFuWindowMessage(int type, string message) : IHUDMessage
{
    public bool IsOpen => Type == 1;
    public bool IsClose => Type == 2;
    public bool IsMessage => Type == 3;

    public string Message { get; } = message;
    private int Type { get; } = type;

    public void Accept(IHUDMessageHandler handler)
    {
        handler.HandleApplyKungFuMessage(this);
    }

    public static ApplyKungFuWindowMessage FromPacket(ApplyKungFuWindowPacket packet)
    {
        return new ApplyKungFuWindowMessage(packet.Type, packet.Msg);
    }
}