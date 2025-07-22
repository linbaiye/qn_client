using QnClient.code.entity;
using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class DropOnEntityInput(long id, int slot) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            DropOnEntityInput = new DropItemOnEntityInputPacket()
            {
                Id = id,
                Slot = slot
            }
        };
    }
}