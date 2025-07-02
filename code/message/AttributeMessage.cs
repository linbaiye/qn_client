using QnClient.code.player.character;
using QnClient.code.ui;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class AttributeMessage : IHUDMessage
{
    
    private AttributeMessage(ValueBar health, ValueBar power, ValueBar innerPower, ValueBar outerPower, int headPercent, int armPercent, int legPercent)
    {
        Health = health;
        Power = power;
        InnerPower = innerPower;
        OuterPower = outerPower;
        HeadPercent = headPercent;
        ArmPercent = armPercent;
        LegPercent = legPercent;
    }

    public void Accept(IHUDMessageHandler handler)
    {
        handler.UpdateAttribute(this);
    }
    
    public ValueBar Health { get; }
    public ValueBar Power{ get; }
    public ValueBar InnerPower{ get; }
    public ValueBar OuterPower{ get; }
    public int HeadPercent { get; }
    public int ArmPercent { get; }
    public int LegPercent { get; }
    

    public static AttributeMessage FromPacket(AttributePacket packet)
    {
        return new AttributeMessage(
            new ValueBar(packet.CurLife, packet.MaxLife),
            new ValueBar(packet.CurPower, packet.MaxPower),
            new ValueBar(packet.CurInnerPower, packet.MaxInnerPower),
            new ValueBar(packet.CurOuterPower, packet.MaxOuterPower),
            packet.HeadPercent, packet.ArmPercent, packet.LegPercent);
    }
}