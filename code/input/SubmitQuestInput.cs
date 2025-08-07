using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class SubmitQuestInput(long id, string name) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            SubmitQuestInput = new SubmitQuestInputPacket()
            {
                Id = id,
                QuestName = name,
            }
        };
    }
}