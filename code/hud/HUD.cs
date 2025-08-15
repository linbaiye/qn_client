using System;
using System.Collections.Generic;
using Godot;
using NLog;
using QnClient.code.entity;
using QnClient.code.entity.@event;
using QnClient.code.hud.assistance;
using QnClient.code.hud.attribute;
using QnClient.code.hud.bank;
using QnClient.code.hud.guild;
using QnClient.code.hud.lefttext;
using QnClient.code.hud.mapview;
using QnClient.code.hud.npc;
using QnClient.code.map;
using QnClient.code.message;
using QnClient.code.network;
using QnClient.code.player;
using QnClient.code.player.character;
using Bottom = QnClient.code.hud.bottom.Bottom;
using Inventory = QnClient.code.hud.inventory.Inventory;
using KungFuBook = QnClient.code.hud.kungfu.KungFuBook;

namespace QnClient.code.hud;

public partial class HUD : CanvasLayer, IHUDMessageHandler
{
    private Bottom _bottom;

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private KungFuBook _kungFuBook;
    private Inventory _inventory;

    private AudioManager _audioManager;
    public event Action<int>? InventoryItemDropped;

    private NpcMainMenu _npcMainMenu;
    
    private NpcTradeMenu _npcTradeMenu;

    private PlayerTradeWindow _playerTradeWindow;

    private Assistance _assistance;

    private LeftTextArea _leftTextArea;

    private system.System _system;

    private LeftUpText _leftUpText;

    private AttributeEquipment _attributeEquipment;

    private Quest _quest;
    
    private Bank _bank;

    private MapView _mapView;

    private TextHistoryWindow _textHistoryWindow;

    private GuildManager _guildManager;

    public override void _Ready()
    {
        _bottom = GetNode<Bottom>("Bottom");
        _bottom.InventoryButtonPressed += OnInventoryPressed;
        _bottom.KungFuBookButtonPressed += OnKungFuBookPressed;
        _bottom.SystemButtonPressed += OnSystemButtonPressed;
        _bottom.AssistanceButtonPressed += OnAssistancePressed;
        _kungFuBook = GetNode<KungFuBook>("KungFuBook");
        _inventory = GetNode<Inventory>("Inventory");
        _inventory.ItemDragReleased += OnInventoryItemDragReleased;
        _audioManager = GetNode<AudioManager>("AudioManager");
        _npcMainMenu = GetNode<NpcMainMenu>("NpcMainMenu");
        _npcTradeMenu = GetNode<NpcTradeMenu>("NpcTradeMenu");
        _npcTradeMenu.SetInputInventory(_inventory, _bottom.DisplayText);
        _playerTradeWindow = GetNode<PlayerTradeWindow>("PlayerTradeWindow");
        _playerTradeWindow.SetInventory(_inventory);
        _bottom.SetBookAndInventory(_kungFuBook, _inventory);
        _assistance = GetNode<Assistance>("Assistance");
        _assistance.SetAttributeProvider(_bottom);
        _leftTextArea = GetNode<LeftTextArea>("LeftTextArea");
        _system = GetNode<system.System>("System");
        _system.SetSettingChangedListener(_audioManager.OnSystemSettingChanged);
        _leftUpText = GetNode<LeftUpText>("LeftUpText");
        _attributeEquipment = GetNode<AttributeEquipment>("AttributeEquipment");
        _quest = GetNode<Quest>("Quest");
        _bank = GetNode<Bank>("Bank");
        _mapView = GetNode<MapView>("MapView");
        _textHistoryWindow = GetNode<TextHistoryWindow>("TextHistoryWindow");
        _bottom.SetTextHistoryWindow(_textHistoryWindow);
        _bottom.PlayerAvatarPressed += _attributeEquipment.OnAvatarPressed;
        Visible = false;
    }
        
    public void CharacterEventHandler(IEntityEvent entityEvent)
    {
        if (entityEvent is EntityChangeCoordinateEvent { Source: ICharacter })
        {
            _bottom.UpdateCoordinate(entityEvent.Source.Coordinate);
            _assistance.OnCharacterCoordinateChanged(entityEvent.Source.Coordinate);
            _mapView.UpdateCharacterCoordinate(entityEvent.Source.Coordinate);
        }
    }

    private void OnInventoryItemDragReleased(int slotNumber)
    {
        if (!_bank.HandleInventoryDragItem(slotNumber))
            InventoryItemDropped?.Invoke(slotNumber);
    }

    private void OnAssistancePressed()
    {
        _assistance.ButtonPressed();
    }

    private void OnInventoryPressed()
    {
        _inventory.OnShortcutButtonClicked();
    }
    
    private void OnKungFuBookPressed()
    {
        _kungFuBook.OnShortcutButtonClicked();
    }


    public void SetConnection(Connection connection)
    {
        foreach (var child in GetChildren())
        {
            if (child is IConnectionAware connectionAware)
                connectionAware.SetConnection(connection);
        }
        _guildManager = new GuildManager(
            GetNode<SimpleInputWindow>("CreateGuildWindow"),
            connection, 
            GetNode<ApplyKungFuForm>("ApplyKungFuForm"));
    }

    public void UpdateKungFuBookView(KungFuBookMessage message)
    {
        _kungFuBook.ShowKungFuBook(message);
    }

    public void KungFuGainExp(string name, int level, bool attack)
    {
        _kungFuBook.KungFuGainExp(name, level);
        _bottom.BlinkKungFu(name);
        if (attack)
            _bottom.UpdateExpBar(level);
    }

    public void BlinkText(string text)
    {
        _bottom.BlinkText(text);
    }

    public void Equip(PlayerEquipMessage message)
    {
        _bottom.Equip(message);
        _attributeEquipment.Equip(message);
    }


    public void Unequip(EquipmentType type)
    {
        _bottom.Unequip(type);
        _attributeEquipment.Unequip(type);
    }

    public void UpdateInventorySlot(InventoryItemMessage message)
    {
        _inventory.UpdateSlot(message);
    }

    public void StartDropItem(string name, int number, int slot, Vector2I coordinate)
    {
        _inventory.StartDropItem(name, number, slot, coordinate);
    }

    public void ShowNpcMenu(NpcMenuMessage message)
    {
        _npcTradeMenu.Visible = false;
        _npcMainMenu.Show(message);
    }

    public void ShowNpcSellMenu(NpcTradeMenuMessage message)
    {
        _npcMainMenu.Visible = false;
        _npcTradeMenu.ShowSellMenu(message);
    }

    public void OpenTradeWindow(OpenPlayerTradeWindowMessage message)
    {
        _playerTradeWindow.OpenWindow(message);
    }

    public void ClosePlayerTradeWindow()
    {
        _playerTradeWindow.Close();
    }

    public void CloseBankWindow()
    {
        _bank.Close();
    }

    public void UpdateTradeWindowSlot(bool self, InventoryItemMessage item)
    {
        _playerTradeWindow.UpdateSlot(self, item);
    }

    public void OnCharacterTeleported(TeleportMessage message)
    {
        _audioManager.PlayBgm(message.Bgm);
        _bottom.OnCharacterTeleported(message.MapTitle, message.Coordinate);
        _mapView.OnPlayerTeleported(message);
    }


    public void FillPills(List<string> pills)
    {
        _assistance.FillPills(pills);
    }

    public void ShowAttributeEquipment(AttributeEquipmentMessage message)
    {
        _attributeEquipment.ShowAttributeEquipment(message);
    }

    public void ShowInventoryItemDescription(int slot, string text)
    {
        _inventory.ShowDescription(slot, text);
    }

    public void ShowKungFuDescription(int slot, string text)
    {
        _kungFuBook.ShowDescription(slot, text);
    }

    public void ShowEquipmentDescription(EquipmentType type, string text)
    {
        _attributeEquipment.ShowEquipmentDescription(type, text);
    }

    public void ShowBankItemDescription(int slot, string text)
    {
        _bank.ShowDescription(slot, text);
    }

    public void ShowQuest(ShowQuestMessage message)
    {
        _quest.Show(message);
    }

    public void ShowBank(ShowBankMessage message)
    {
        _bank.Show(message, _inventory);
    }

    public void ShowCoordinateNameOnMap(InteractablePositionNameMessage message)
    {
        _mapView.ShowCoordinateAndName(message);
    }

    public void ShowCreateGuildWindow(int slotId, string title, string tip)
    {
        _guildManager.ShowCreateGuildWindow(slotId, title, tip);
    }

    public void HandleApplyKungFuMessage(ApplyKungFuWindowMessage message)
    {
        _guildManager.HandleApplyKungFuMessage(message);
    }

    public void DisplayBottomText(string text, string color, string bgColor)
    {
        _bottom.DisplayText(text, color, bgColor);
    }

    public void DisplayBottomText(TextMessage message)
    {
        _bottom.DisplayText(message);
    }

    public void DisplayLeftText(string text)
    {
        _leftTextArea.Display(text);
    }

    public void DisplayLeftUpText(string text)
    {
        _leftUpText.Display(text);
    }

    public void UpdateInventoryView(InventoryMessage message)
    {
        _inventory.UpdateInventoryView(message);
    }

    public void UpdateAttribute(AttributeMessage message)
    {
        _bottom.UpdateAttribute(message);
    }

    public void OnCharacterJoined(JoinRealmMessage message)
    {
        FileStorage.Space = message.Id.ToString();
        foreach (var child in GetChildren())
        {
            if (child is ICharacterJoinedAware characterJoinedAware)
                characterJoinedAware.OnCharacterJoined(message);
        }
        Visible = true;
    }

    public void SetMap(IMap map)
    {
        _mapView.SetMap(map);
    }

    public void UpdateActiveKungFuList(SyncActiveKungFuListMessage message)
    {
        _bottom.SyncActiveKungFuList(message);
        _bottom.UpdateExpBar(message.AttackLevel);
        _attributeEquipment.KungFuRefreshed();
    }

    public void PlaySound(string entityName, string soundName)
    {
        _audioManager.PlaySound(soundName);
    }

    public void UpdateLifeBars(PlayerDamagedMessage message)
    {
        _bottom.UpdateLifeBars(message);
    }
    
    public void SetItemFilter(Func<IEnumerable<GroundItem>> action)
    {
        _assistance.SetItemFilter(action);
    }
    
    private void OnSystemButtonPressed()
    {
        _system.OnButtonPressed();
    }
}