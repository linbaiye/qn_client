using Godot;
using QnClient.code.entity;
using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public readonly struct MoveInput(CreatureDirection direction, Vector2I coordinate) : I2ServerMessage
{
    public CreatureDirection Direction { get; } = direction;
    
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            MoveInput = new MoveInputPacket()
            {
                X = coordinate.X,
                Y = coordinate.Y,
                Direction = (int)Direction,
            }
        };
    }
}