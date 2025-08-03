using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class ChatInput(string text) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            Chat = new ChatInputPacket()
            {
                Text = text
            }
        };
    }
}