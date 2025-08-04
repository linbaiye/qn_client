using QnClient.code.network.toserver;
using QnClient.code.player;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class ClickEquipmentInput(EquipmentType type) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            ClickEquipment = new ClickEquipmentInputPacket()
            {
                EquipType = (int)type
            }
        };
    }
}