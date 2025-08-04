using System.Collections.Generic;
using System.Linq;
using QnClient.code.hud;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class AttributeEquipmentMessage(string[] attributes, string name, string age,
    bool male, List<PlayerEquipMessage> msg, bool quietly) : IHUDMessage
{
    public string[] Attributes { get; } = attributes;

    public string Name { get; } = name;
    public string Age { get; } = age;
    
    public bool Male { get; } = male;

    public bool Quietly { get; } = quietly;

    public List<PlayerEquipMessage> Equipments { get; set; } = msg;
    
    public void Accept(IHUDMessageHandler handler)
    {
        handler.ShowAttributeEquipment(this);
    }

    private static List<PlayerEquipMessage> CreateEquipments(IEnumerable<PlayerEquipPacket> packets)
    {
        List<PlayerEquipMessage> equipmentMessages = new List<PlayerEquipMessage>();
        foreach (var itemPacket in packets)
        {
            equipmentMessages.Add(PlayerEquipMessage.FromPacket(itemPacket));
        }
        return equipmentMessages;
    }
    
    public static AttributeEquipmentMessage FromPacket(AttributeEquipPacket packet)
    {
        var attributes = packet.Attributes.ToArray();
        return new AttributeEquipmentMessage(attributes, packet.Name, packet.Age, packet.Male, CreateEquipments(packet.Equipments), packet.Quietly);
    }
}