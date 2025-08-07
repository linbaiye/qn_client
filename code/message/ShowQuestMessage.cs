using QnClient.code.hud;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class ShowQuestMessage(long id, string npcName, string questName, string abstraction, string description, string submitText) : IHUDMessage
{
    public long Id => id;
    
    public string NpcName => npcName;
    
    public string Abstraction => abstraction;
    
    public string Description => description;

    public string QuestName => questName;
    public string SubmitText => submitText;
    public void Accept(IHUDMessageHandler handler)
    {
        handler.ShowQuest(this);
    }

    public static ShowQuestMessage FromPacket(ShowQuestPacket packet)
    {
        return new ShowQuestMessage(packet.Id, packet.NpcName,packet.Quest, packet.Abstraction, packet.Description, packet.Submit);
    }
}