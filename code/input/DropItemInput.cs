using Godot;
using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class DropItemInput(int slot, Vector2I coordinate) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            DropInput = new DropItemInputPacket()
            {
                Slot = slot,
                X = coordinate.X,
                Y = coordinate.Y,
            }
        };
    }
}