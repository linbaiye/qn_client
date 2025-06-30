using System.Collections.Generic;
using Godot;
using QnClient.code.player.character;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class JoinRealmMessage 
{
    public string MapFile { get; private init; }
    public string ResourceName { get; private init; }
    
    public bool Male { get; private init; }
    
    public ValueBar LifeBar { get; private init; }
    public ValueBar ArmLifeBar { get; private init; }
    public ValueBar HeadLifeBar { get; private init; }
    public ValueBar LegLifeBar { get; private init; }
    public ValueBar InnerPowerBar { get; private init; }
    public ValueBar OuterPowerBar { get; private init; }
    public ValueBar PowerBar { get; private init; }
    public string ? AttackKungFu { get; private init; }
    
    public long Id { get; private init; }
    
    public Vector2I Coordinate { get; private init; }
    
    public string Name { get; private init; }
    
    public List<PlayerEquipMessage> Equipments { get;private init; }
    
    public static JoinRealmMessage Parse(JoinRealmPacket joinPacket)
    {
        var map = joinPacket.Teleport.Map.Replace(".map", "");
        var resource = joinPacket.Teleport.Resource.Replace(".zip", "");
        List<PlayerEquipMessage> equipMessages = new List<PlayerEquipMessage>();
        foreach (var eq in joinPacket.Equipments)
        {
            equipMessages.Add(PlayerEquipMessage.FromPacket(eq));
        }
        return new JoinRealmMessage()
        {
            MapFile = map,
            ResourceName = resource,
            Coordinate = new Vector2I(joinPacket.Teleport.X, joinPacket.Teleport.Y),
            Name = joinPacket.Name,
            Id = joinPacket.Id,
            Male = joinPacket.Male,
            LifeBar = new ValueBar(joinPacket.Attribute.CurLife, joinPacket.Attribute.MaxLife),
            PowerBar = new ValueBar(joinPacket.Attribute.CurPower, joinPacket.Attribute.MaxPower),
            InnerPowerBar = new ValueBar(joinPacket.Attribute.CurInnerPower, joinPacket.Attribute.MaxInnerPower),
            OuterPowerBar = new ValueBar(joinPacket.Attribute.CurOuterPower, joinPacket.Attribute.MaxOuterPower),
            ArmLifeBar= new ValueBar(joinPacket.Attribute.ArmPercent, 100),
            HeadLifeBar= new ValueBar(joinPacket.Attribute.HeadPercent, 100),
            LegLifeBar= new ValueBar(joinPacket.Attribute.LegPercent, 100),
            AttackKungFu = joinPacket.AttackKungFu,
            Equipments = equipMessages,
            //ProtectionKungFu = joinPacket.HasProtectionKungFu? new KungFu(joinPacket.ProtectionKungFu) : null,
            //FootKungFu = joinPacket.HasFootKungFuName ? new FootKungFu(joinPacket.FootKungFuName, joinPacket.FootKungFuCanFly) : null,
            //AssistantKungFu= joinPacket.HasAssistantKungFu ? new KungFu(joinPacket.AssistantKungFu) : null,
        };
    }
}