using QnClient.code.hud;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class OpenPlayerTradeWindowMessage(bool proactive, string selfName, string anotherName, int slot, string itemName, long maxNumber) : IHUDMessage
{
    public bool Proactive { get; } = proactive;
    
    public string SelfName { get; } = selfName;
    
    public string AnotherName { get; } = anotherName;
    
    public int Slot { get; } = slot;
    
    public string ItemName { get; } = itemName;
    
    public long MaxNumber { get; } = maxNumber;
    
    public void Accept(IHUDMessageHandler handler)
    {
        handler.OpenTradeWindow(this);
    }

    public static OpenPlayerTradeWindowMessage FromPacket(OpenTradeWindowPacket packet)
    {
        return new OpenPlayerTradeWindowMessage(!packet.Passive, packet.SelfName, packet.AnotherName, packet.Slot, packet.ItemName, packet.MaxNumber);
    }
}