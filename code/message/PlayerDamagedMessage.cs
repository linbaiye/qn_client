using QnClient.code.hud;
using QnClient.code.player.character;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class PlayerDamagedMessage(long id, ValueBar life, int head, int arm, int leg) : ICharacterMessage, IHUDMessage
{
    public long Id { get; } = id;

    public ValueBar LifeBar { get; } = life;

    public int Head { get; } = head;
    public int Arm { get; } = arm;
    public int Leg { get; } = leg;
    public void Accept(ICharacterMessageHandler handler)
    {
        handler.ShowLifeBar(life.Percent);
    }

    public void Accept(IHUDMessageHandler handler)
    {
    }

    public static PlayerDamagedMessage FromPacket(PlayerDamagedPacket packet)
    {
        return new PlayerDamagedMessage(packet.Id, new ValueBar(packet.CurLife, packet.MaxLife), packet.HeadPercent,
            packet.ArmPercent, packet.LegPercent);
    }
}