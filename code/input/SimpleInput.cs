using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public readonly struct SimpleInput(SimpleInput.RequestType type) : I2ServerMessage
{
    public enum RequestType
    {
        KungFuBook = 1,
        Inventory = 2,
        KeyF2 = 3,
        KeyF3 = 4,
        KeyF4 = 5,
        KungFuBookQuietly = 6,
        InventoryQuietly = 7,
        GetPills = 8,
        AttributeEquipment = 9,
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
    public static readonly SimpleInput KungFuBookQuietly = new(RequestType.KungFuBookQuietly);
    
    public static readonly SimpleInput Inventory = new(RequestType.Inventory);
    public static readonly SimpleInput InventoryQuietly  = new(RequestType.InventoryQuietly);
    
    public static readonly SimpleInput F2 = new(RequestType.KeyF2);
    public static readonly SimpleInput F3 = new(RequestType.KeyF3);
    public static readonly SimpleInput F4 = new(RequestType.KeyF4);
    
    public static readonly SimpleInput GetPills = new(RequestType.GetPills);
    public static readonly SimpleInput AttributeEquipment = new(RequestType.AttributeEquipment);
}