using Godot;
using QnClient.code.player.character;
using QnClient.code.player.character.kungfu;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class JoinRealmMessage : AbstractCreatureSnapshot
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
    
    public KungFu? AttackKungFu { get; private init; }
    public FootKungFu? FootKungFu { get; private init; }
    public KungFu? ProtectionKungFu { get; private init; }
    public KungFu? AssistantKungFu { get; private init; }
    
    public static JoinRealmMessage Parse(LoginPacket loginPacket)
    {
        var map = loginPacket.Teleport.Map.Replace(".map", "");
        var resource = loginPacket.Teleport.Resource.Replace(".zip", "");
        return new JoinRealmMessage()
        {
            MapFile = map,
            ResourceName = resource,
            Coordinate = new Vector2I(loginPacket.Teleport.X, loginPacket.Teleport.Y),
            Name = loginPacket.Info.Name,
            Id = loginPacket.Info.Id,
            Male = loginPacket.Info.Male,
            LifeBar = new ValueBar(loginPacket.Attribute.CurLife, loginPacket.Attribute.MaxLife),
            PowerBar = new ValueBar(loginPacket.Attribute.CurPower, loginPacket.Attribute.MaxPower),
            InnerPowerBar = new ValueBar(loginPacket.Attribute.CurInnerPower, loginPacket.Attribute.MaxInnerPower),
            OuterPowerBar = new ValueBar(loginPacket.Attribute.CurOuterPower, loginPacket.Attribute.MaxOuterPower),
            ArmLifeBar= new ValueBar(loginPacket.Attribute.ArmPercent, 100),
            HeadLifeBar= new ValueBar(loginPacket.Attribute.HeadPercent, 100),
            LegLifeBar= new ValueBar(loginPacket.Attribute.LegPercent, 100),
            AttackKungFu = new KungFu(loginPacket.AttackKungFuName),
            ProtectionKungFu = loginPacket.HasProtectionKungFu? new KungFu(loginPacket.ProtectionKungFu) : null,
            FootKungFu = loginPacket.HasFootKungFuName ? new FootKungFu(loginPacket.FootKungFuName, loginPacket.FootKungFuCanFly) : null,
            AssistantKungFu= loginPacket.HasAssistantKungFu ? new KungFu(loginPacket.AssistantKungFu) : null,
        };
    }
}