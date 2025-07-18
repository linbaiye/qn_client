using Godot;
using QnClient.code.message;
using QnClient.code.player;

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

    void UpdateLifeBars(PlayerDamagedMessage message);

    void KungFuGainExp(string name, int level);

    void BlinkText(string text);

    void Equip(EquipmentType type, string prefix, string name, int color = 0, string pairedPrefix = null);
    
    void Unequip(EquipmentType type);

    void UpdateInventorySlot(InventoryItemMessage message);

    void StartDropItem(string name, int number, int slot, Vector2I coordinate);
}