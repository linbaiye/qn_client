using System.Linq;
using QnClient.code.hud;
using QnClient.code.player;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class AttributeEquipmentMessage(string[] attributes, string name, string age) : IHUDMessage
{
    public string[] Attributes { get; set; } = attributes;

    public string Name { get; set; } = name;
    public string Age { get; set; } = age;
    public void Accept(IHUDMessageHandler handler)
    {
        handler.ShowAttributeEquipment(this);
    }
    
    public class EquipmentMessage
    {
        public int Color { get; }
        
        public int Icon { get; }
        
        public string Name { get; }
        
        public EquipmentType EquipmentType { get; }
    }
    
    public static AttributeEquipmentMessage FromPacket(AttributeEquipPacket packet)
    {
        var attributes = packet.Attributes.ToArray();
        return new AttributeEquipmentMessage(attributes, packet.Name, packet.Age);
    }
}