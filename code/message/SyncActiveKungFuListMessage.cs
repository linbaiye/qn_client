using QnClient.code.hud;
using QnClient.code.player.character;
using QnClient.code.player.character.kungfu;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class SyncActiveKungFuListMessage : ICharacterMessage, IHUDMessage
{
    public long Id { get; private set; }
    
    public FootKungFu? FootKungFu { get; private set; }
    
    public string AttackKungFu { get; private set; }
    
    public string ProtectionKungFu { get; private set; }
    
    public string AssistantKungFu { get; private set; }
    
    public string BreathKungFu { get; private set; }
    
    public void Accept(ICharacterMessageHandler handler)
    {
        handler.SyncActiveKungFuList(this);
    }

    public static SyncActiveKungFuListMessage FromPacket(SyncActiveKungFuPacket packet)
    {
        return new SyncActiveKungFuListMessage()
        {
            Id = packet.Id,
            FootKungFu = packet.HasFootKungFu ? new FootKungFu(packet.FootKungFu, packet.FootKungFuCanFly) : null,
            AttackKungFu = packet.AttackKungFu,
            ProtectionKungFu = packet.HasProtectionKungFu ? packet.ProtectionKungFu : "",
            AssistantKungFu = packet.HasAssistantKungFu ? packet.AssistantKungFu : "",
            BreathKungFu = packet.HasBreathKungFu ? packet.BreathKungFu : "",
        };
    }

    public void Accept(IHUDMessageHandler handler)
    {
        handler.UpdateActiveKungFuList(this);
    }
}