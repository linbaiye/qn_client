using QnClient.code.hud;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class NpcSellMenuMessage : IHUDMessage
{
    public long Id { get; set; }
    public void Accept(IHUDMessageHandler handler)
    {
        throw new System.NotImplementedException();
    }

    public static NpcSellMenuMessage FromPacket(NpcSellMenuPacket packet)
    {
        
    }
}