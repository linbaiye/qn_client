using QnClient.code.network.toserver;
using QnClient.code.player;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public record UnequipInput(EquipmentType type) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            UnequipInput = new UnequipInputPacket()
            {
                Type = (int)type,
            }
        };
    }
}