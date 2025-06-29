using Godot;
using NLog;
using QnClient.code.entity;
using QnClient.code.entity.@event;
using QnClient.code.message;
using QnClient.code.network;
using QnClient.code.player.character;
using QnClient.code.ui.bottom;
using QnClient.code.ui.inventory;
using QnClient.code.ui.kungfu;

namespace QnClient.code.ui;

public partial class HUD : CanvasLayer, IHUDMessageHandler
{
    private Bottom _bottom;

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private KungFuBook _kungFuBook;
    private Inventory _inventory;

    private Connection _connection;

    
    public override void _Ready()
    {
        _bottom = GetNode<Bottom>("Bottom");
        _bottom.InventoryButtonPressed += OnInventoryPressed;
        _bottom.KungFuBookButtonPressed += OnKungFuBookPressed;
        _bottom.SystemButtonPressed += OnSystemButtonPressed;
        _kungFuBook = GetNode<KungFuBook>("KungFuBook");
        _inventory = GetNode<Inventory>("Inventory");
        _inventory.ItemDragRelesed += OnInventoryItemDragReleased;
        Visible = false;
    }

    public void CharacterEventHandler(IEntityEvent entityEvent)
    {
        if (entityEvent is CharacterEvent characterEvent)
        {
            _bottom.Handle(characterEvent);
            if (characterEvent.Type == CharacterEvent.EventType.Joined)
            {
                Visible = true;
            }
        }
        else if (entityEvent is EntityChangeCoordinateEvent { Source: ICharacter })
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

    public void OnSystemButtonPressed()
    {
        GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
    }
}