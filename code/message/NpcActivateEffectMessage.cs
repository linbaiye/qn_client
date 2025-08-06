using QnClient.code.entity;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class NpcActivateEffectMessage(long id, string effect, int millis) : INpcMessage
{
    public long Id { get; } = id;
    private float Seconds { get; } = (float)millis / 1000;
    public void Accept(INpcMessageHandler handler)
    {
        handler.ActivateEffect(Seconds);
    }

    public static NpcActivateEffectMessage FromPacket(ActivateEffectPacket packet)
    {
        return new NpcActivateEffectMessage(packet.Id, packet.Effect, packet.Millis);
    }
}