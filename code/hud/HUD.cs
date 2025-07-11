using Godot;
using NLog;
using QnClient.code.entity;
using QnClient.code.entity.@event;
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

    public override void _Ready()
    {
        _bottom = GetNode<Bottom>("Bottom");
        _bottom.InventoryButtonPressed += OnInventoryPressed;
        _bottom.KungFuBookButtonPressed += OnKungFuBookPressed;
        _bottom.SystemButtonPressed += OnSystemButtonPressed;
        _kungFuBook = GetNode<KungFuBook>("KungFuBook");
        _inventory = GetNode<Inventory>("Inventory");
        _inventory.ItemDragRelesed += OnInventoryItemDragReleased;
        _audioManager = GetNode<AudioManager>("AudioManager");
        _bottom.UnequipPressed += UnequipPressed;
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
        _inventory.OnShortcutButtonClicked(_connection);
    }
    
    private void OnKungFuBookPressed()
    {
        _kungFuBook.OnShortcutButtonClicked(_connection);
    }


    public void SetConnection(Connection connection)
    {
        _connection = connection;
    }

    public void UpdateKungFuBookView(KungFuBookMessage message)
    {
        _kungFuBook.ShowKungFuBook(message, _connection);
    }

    public void KungFuGainExp(string name, int level)
    {
        _kungFuBook.KungFuGainExp(name, level);
        _bottom.BlinkKungFu(name);
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
    }

    public void DisplayText(string text)
    {
        _bottom.DisplayText(text);
    }

    private void OnInventoryItemDragReleased(int number)
    {
        Logger.Debug("Dragged inventory slot {}", number);
    }

    public void UpdateInventoryView(InventoryMessage message)
    {
        _inventory.UpdateInventoryView(message, _connection);
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
    }

    public void PlaySound(string entityName, string soundName)
    {
        _audioManager.PlaySound(entityName, soundName);
    }

    public void UpdateLifeBars(PlayerDamagedMessage message)
    {
        _bottom.UpdateLifeBars(message);
    }

    public void OnSystemButtonPressed()
    {
        GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
    }
}