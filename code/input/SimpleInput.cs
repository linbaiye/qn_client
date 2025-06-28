using Godot;
using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class SimpleInput(SimpleInput.RequestType type) : I2ServerMessage
{
    public enum RequestType
    {
        KungFuBook = 1,
        Inventory = 2,
    }

    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            SimpleInput = new SimpleInputPacket()
            {
                Type = (int)type,
            }
        };
    }

    public static readonly SimpleInput KungFuBook = new(RequestType.KungFuBook);
    
    public static readonly SimpleInput Inventory = new(RequestType.Inventory);
}