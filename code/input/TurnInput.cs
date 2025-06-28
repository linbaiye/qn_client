using QnClient.code.entity;
using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public readonly struct TurnInput(CreatureDirection direction) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            TurnInput = new TurnInputPacket()
            {
                Direction = (int)direction
            }
        };
    }
}