using Godot;
using QnClient.code.entity;
using QnClient.code.player;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class MoveMessage(Vector2I from, CreatureDirection direction, long id, MoveAction? action = null) : AbstractEntityMessage(id),
    IPlayerMessage, INpcMessage
{

    public Vector2I From { get; } = from;
    public CreatureDirection Direction { get; } = direction;
    
    public MoveAction? Action { get; } = action;

    public void Accept(IPlayerMessageHandler messageHandler)
    {
        messageHandler.Move(this);
    }

    public void Accept(INpcMessageHandler handler)
    {
        handler.Move(this);
    }

    public override string ToString()
    {
        return $"{nameof(From)}: {From}, {nameof(Direction)}: {Direction}";
    }


    public static MoveMessage FromPacket(MonsterMoveEventPacket packet)
    {
        return new MoveMessage(new Vector2I(packet.X, packet.Y), (CreatureDirection)packet.Direction, packet.Id) ;
    }

}