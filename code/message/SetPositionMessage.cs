using Godot;
using QnClient.code.entity;
using QnClient.code.player;
using QnClient.code.player.character;

namespace QnClient.code.message;

public readonly struct SetPositionMessage(long id, Vector2I coordinate, CreatureDirection direction, PlayerState state) : INpcMessage, IPlayerMessage, ICharacterMessage
{
    public long Id => id;
    public void Accept(ICharacterMessageHandler handler)
    {
        handler.SetPosition(Coordinate, state, direction);
    }

    public void Accept(IPlayerMessageHandler handler)
    {
        handler.SetPosition(this);
    }

    public void Accept(INpcMessageHandler handler)
    {
        handler.SetPosition(this);
    }

    public Vector2I Coordinate => coordinate;
    
    public CreatureDirection Direction => direction;

    public PlayerState State => state;
}