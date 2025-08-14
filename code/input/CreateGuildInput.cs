using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class CreateGuildInput(bool confirmed, string name, int fromSlot) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            CreateGuildInput = new CreateGuildInputPacket()
            {
                Confirm = confirmed,
                FromSlot = fromSlot,
                Name = name,
            }
        };
    }

    public static CreateGuildInput Cancel()
    {
        return new CreateGuildInput(false, "", 0);
    }

    public static CreateGuildInput Confirm(string name, int fromSlot)
    {
        return new CreateGuildInput(true, name, fromSlot);
    }
}