using System;
using Godot;
using NLog;
using QnClient.code.entity;
using QnClient.code.entity.@event;
using QnClient.code.hud.npc;
using QnClient.code.input;
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

    private Connection _connection;
    
    private AudioManager _audioManager;
    public event Action<int>? InventoryItemDropped;

    private NpcMainMenu _npcMainMenu;
    
    private NpcTradeMenu _npcTradeMenu;

    private PlayerTradeWindow _playerTradeWindow;

    public override void _Ready()
    {
        _bottom = GetNode<Bottom>("Bottom");
        _bottom.InventoryButtonPressed += OnInventoryPressed;
        _bottom.KungFuBookButtonPressed += OnKungFuBookPressed;
        _bottom.SystemButtonPressed += OnSystemButtonPressed;
        _kungFuBook = GetNode<KungFuBook>("KungFuBook");
        _inventory = GetNode<Inventory>("Inventory");
        _inventory.ItemDragReleased += s => InventoryItemDropped?.Invoke(s);
        _audioManager = GetNode<AudioManager>("AudioManager");
        _bottom.UnequipPressed += UnequipPressed;
        _npcMainMenu = GetNode<NpcMainMenu>("NpcMainMenu");
        _npcTradeMenu = GetNode<NpcTradeMenu>("NpcTradeMenu");
        _npcTradeMenu.SetInputInventory(_inventory, _bottom.DisplayText);
        _playerTradeWindow = GetNode<PlayerTradeWindow>("PlayerTradeWindow");
        _playerTradeWindow.SetInventory(_inventory);
        _bottom.SetBookAndInventory(_kungFuBook, _inventory);
        Visible = false;
    }

    private void UnequipPressed(EquipmentType t)
    {
        _connection.WriteAndFlush(new UnequipInput(t));
    }
        
    public void CharacterEventHandler(IEntityEvent entityEvent)
    {
        if (entityEvent is EntityChangeCoordinateEvent { Source: ICharacter })
        {
            _bottom.UpdateCoordinate(entityEvent.Source.Coordinate);
        }
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
        _connection = connection;
        foreach (var child in GetChildren())
        {
            if (child is IConnectionAware connectionAware)
                connectionAware.SetConnection(connection);
        }
    }

    public void UpdateKungFuBookView(KungFuBookMessage message)
    {
        _kungFuBook.ShowKungFuBook(message);
    }

    public void KungFuGainExp(string name, int level)
    {
        _kungFuBook.KungFuGainExp(name, level);
        _bottom.BlinkKungFu(name);
        _bottom.UpdateExpBar(level);
    }

    public void BlinkText(string text)
    {
        _bottom.BlinkText(text);
    }

    public void Equip(EquipmentType type, string prefix, string name, int color = 0, string pairedPrefix = null)
    {
        _bottom.Equip(type, prefix, name, color, pairedPrefix);
    }

    public void Unequip(EquipmentType type)
    {
        _bottom.Unequip(type);
    }

    public void UpdateInventorySlot(InventoryItemMessage message)
    {
        _inventory.UpdateSlot(message);
        _bottom.OnInventorySlotUpdated(message);
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

    public void CloseTradeWindow()
    {
        _playerTradeWindow.Close();
    }

    public void UpdateTradeWindowSlot(bool self, InventoryItemMessage item)
    {
        _playerTradeWindow.UpdateSlot(self, item);
    }

    public void OnCharacterTeleported(TeleportMessage message)
    {
        _audioManager.PlayBgm(message.Bgm);
        _bottom.OnCharacterTeleported(message.MapTitle, message.Coordinate);
    }

    public void DisplayText(string text)
    {
        _bottom.DisplayText(text);
    }

    public void UpdateInventoryView(InventoryMessage message)
    {
        _inventory.UpdateInventoryView(message);
        _bottom.OnInventoryUpdated(message);
    }

    public void UpdateAttribute(AttributeMessage message)
    {
        _bottom.UpdateAttribute(message);
    }

    public void OnCharacterJoined(JoinRealmMessage message)
    {
        _bottom.OnCharacterJoined(message);
        _audioManager.PlayBgm(message.Bgm);
        Visible = true;
    }

    public void UpdateActiveKungFuList(SyncActiveKungFuListMessage message)
    {
        _bottom.SyncActiveKungFuList(message);
        _bottom.UpdateExpBar(message.AttackLevel);
    }

    public void PlaySound(string entityName, string soundName)
    {
        _audioManager.PlaySound(soundName);
    }

    public void UpdateLifeBars(PlayerDamagedMessage message)
    {
        _bottom.UpdateLifeBars(message);
    }



    private void OnSystemButtonPressed()
    {
        GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
    }
}