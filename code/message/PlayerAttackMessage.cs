using QnClient.code.entity;
using QnClient.code.player;
using QnClient.code.player.character;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public readonly struct PlayerAttackMessage(long id, AttackAction action, CreatureDirection dir, string effect) : IPlayerMessage, ICharacterMessage
{

    public long Id { get; } = id;
    public void Accept(ICharacterMessageHandler handler)
    {
        handler.Attack(action, dir, effect);
    }

    public void Accept(IPlayerMessageHandler handler)
    {
        handler.Attack(action, dir, effect);
    }

    public static PlayerAttackMessage FromPacket(PlayerAttackPacket packet)
    {
        return new PlayerAttackMessage(packet.Id, (AttackAction)packet.Action, (CreatureDirection)packet.Direction, packet.EffectSprite);
    }

}