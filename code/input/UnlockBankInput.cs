using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class UnlockBankInput(long id) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            UnlockBank = new UnlockBankInputPacket()
            {
                NpcId = id,
            }
        };
    }
}