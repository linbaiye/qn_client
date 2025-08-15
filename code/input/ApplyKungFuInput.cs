using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class ApplyKungFuInput : I2ServerMessage
{
    public string? Name { get; init; } = "";
    public int Speed {get; init;}
    public int Recovery {get; init;}
    public int Avoid {get; init;}
    public int HeadDamage {get; init;}
    public int ArmDamage {get; init;}
    public int BodyDamage {get; init;}
    public int LegDamage {get; init;}
    public int HeadArmor {get; init;}
    public int ArmArmor {get; init;}
    public int BodyArmor {get; init;}
    public int LegArmor {get; init;}
    public int PowerToSwing {get; init;}
    public int InnerPowerToSwing {get; init;}
    public int OuterPowerToSwing {get; init;}
    public int LifeToSwing {get; init;}
    public int Type {get; init;}

    public ClientPacket ToPacket()
    {
        return new ClientPacket() {
            CreateGuildKungFu = new ClientCreateGuildKungFuPacket() {
                Name = Name,
                AttackSpeed = Speed,
                Recovery = Recovery,
                Avoidance = Avoid,
                Type = (int)Type,
                BodyDamage = BodyDamage,
                HeadDamage = HeadDamage,
                LegDamage = LegDamage,
                ArmDamage = ArmDamage,
                BodyArmor = BodyArmor,
                HeadArmor = HeadArmor,
                ArmArmor = ArmArmor,
                LegArmor = LegArmor,
                Life = LifeToSwing,
                Power = PowerToSwing,
                InnerPower = InnerPowerToSwing,
                OuterPower = OuterPowerToSwing,
            }
        };
    }
}