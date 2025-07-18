using Godot;
using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class ConfirmDropItemInput(int slot, int number, Vector2I coordinate) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            ConfirmDropInput = new ConfirmDropItemInputPacket()
            {
                Slot = slot,
                Number = number,
                X = coordinate.X,
                Y = coordinate.Y,
            }
        };
    }
}