using Godot;
using QnClient.code.entity;
using QnClient.code.network.toserver;
using QnClient.code.util;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public readonly struct MoveInput(CreatureDirection direction, Vector2I from) : I2ServerMessage
{
    public CreatureDirection Direction { get; } = direction;

    public Vector2 Destination => from.Move(Direction).ToPosition();

    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            MoveInput = new MoveInputPacket()
            {
                X = from.X,
                Y = from.Y,
                Direction = (int)Direction,
            }
        };
    }
}