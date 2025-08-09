using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class RealmInput(int t) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            RealmInput = new RealmInputPacket()
            {
                Type = t,
            }
        };
    }

    public static readonly RealmInput GetNpcCoordinates = new RealmInput(1);
}