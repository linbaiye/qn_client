using QnClient.code.message;

namespace QnClient.code.hud;

public interface IHUDMessageHandler
{

    void UpdateKungFuBookView(KungFuBookMessage message);

    void DisplayText(string text);

    void UpdateInventoryView(InventoryMessage message);

    void UpdateAttribute(AttributeMessage message);

    void OnCharacterJoined(JoinRealmMessage message);
    
    void UpdateActiveKungFuList(SyncActiveKungFuListMessage message);
    
    void PlaySound(string entityName, string soundName);

}