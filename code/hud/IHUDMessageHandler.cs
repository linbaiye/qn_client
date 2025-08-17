using System.Collections.Generic;
using Godot;
using QnClient.code.message;
using QnClient.code.player;

namespace QnClient.code.hud;

public interface IHUDMessageHandler
{
    void UpdateKungFuBookView(KungFuBookMessage message);

    void DisplayBottomText(string text, string color, string bgColor);

    void DisplayBottomText(TextMessage message);
    
    void DisplayLeftText(string text);
    
    void DisplayLeftUpText(string text);

    void UpdateInventoryView(InventoryMessage message);

    void UpdateAttribute(AttributeMessage message);

    void OnCharacterJoined(JoinRealmMessage message);
    
    void UpdateActiveKungFuList(SyncActiveKungFuListMessage message);
    
    void PlaySound(string soundName);

    void UpdateLifeBars(PlayerDamagedMessage message);

    void KungFuGainExp(string name, int level, bool attack);

    void BlinkText(string text);

    void Equip(PlayerEquipMessage message);
    
    void Unequip(EquipmentType type);

    void UpdateInventorySlot(InventoryItemMessage message);

    void StartDropItem(string name, int number, int slot, Vector2I coordinate);
    void ShowNpcMenu(NpcMenuMessage message);

    void ShowNpcSellMenu(NpcTradeMenuMessage message);
    
    void OpenTradeWindow(OpenPlayerTradeWindowMessage message);

    void ClosePlayerTradeWindow();
    
    void CloseBankWindow();

    void UpdateTradeWindowSlot(bool self, InventoryItemMessage item);

    void OnCharacterTeleported(TeleportMessage message);

    void FillPills(List<string> pills);
    
    void ShowAttributeEquipment(AttributeEquipmentMessage message);

    void ShowInventoryItemDescription(int slot, string text);
    
    void ShowKungFuDescription(int slot, string text);
    
    void ShowEquipmentDescription(EquipmentType type, string text);

    void ShowBankItemDescription(int slot, string text);
    
    void ShowQuest(ShowQuestMessage message);

    void ShowBank(ShowBankMessage message);
    
    void ShowCoordinateNameOnMap(InteractablePositionNameMessage message);

    void ShowCreateGuildWindow(int slotId, string title, string tip);
    void HandleApplyKungFuMessage(ApplyKungFuWindowMessage message);
}