using QnClient.code.entity;
using QnClient.code.player;
using QnClient.code.player.character;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class ProjectileFiredMessage(long id, long targetId, string sprite, int flyMillis) : IPlayerMessage, ICharacterMessage, INpcMessage
{
    public long Id { get; } = id;
    private long TargetId { get; } = targetId;

    private string Sprite { get; } = sprite;

    private int FlyMillis { get; } = flyMillis;
    
    public void Accept(ICharacterMessageHandler handler)
    {
        handler.FireProjectile(TargetId, Sprite, FlyMillis);
    }

    public void Accept(IPlayerMessageHandler handler)
    {
        handler.FireProjectile(TargetId, Sprite, FlyMillis);
    }

    public static ProjectileFiredMessage FromPacket(ProjectilePacket packet)
    {
        return new ProjectileFiredMessage(packet.Id, packet.TargetId, packet.Sprite, packet.FlyingTimeMillis);
    }
    
    public void Accept(INpcMessageHandler handler)
    {
        handler.FireProjectile(TargetId, Sprite, FlyMillis);
    }
}