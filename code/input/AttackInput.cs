using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public struct AttackInput(long id) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket
        {
            AttackInput = new AttackInputPacket
            {
                Id = id,
            }
        };
    }
}