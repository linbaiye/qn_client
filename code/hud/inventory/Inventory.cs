using System;
using Godot;
using NLog;
using QnClient.code.input;
using QnClient.code.message;
using QnClient.code.network;
using QnClient.code.sprite;

namespace QnClient.code.hud.inventory;

public partial class Inventory : AbstractSlotContainer
{
    
    private readonly ILogger Log = LogManager.GetCurrentClassLogger();
    
    private readonly ZipFileSpriteLoader _zipFileSpriteLoader = ZipFileSpriteLoader.Instance;

    private Texture2D[] _icons;

    private Connection _connection;
    
    private InventoryMessage _message;
    public event Action<int>? ItemDragReleased;
    public override void _Ready()
    {
        base._Ready();
        _icons = _zipFileSpriteLoader.LoadOrderedItemIcons();
    }

    protected override Slot CreateSlot(string name)
    {
        return Slot.Create(name, new Vector2(40, 40), new Vector2(30, 30), false);
    }

    protected override void OnSlotLeftMouseButtonReleased(int number)
    {
        var slot = FindSlotHasHovering();
        if (slot != null && slot.Number != number)
        {
            _connection.WriteAndFlush(SwapSlotInput.Inventory(number, slot.Number));
            return;
        }
        if (slot != null && slot.Number == number)
        {
            return;
        }
        foreach (var messageItem in _message.Items)
        {
            if (messageItem.Slot == number)
            {
                ItemDragReleased?.Invoke(number);
                break;
            }
        }
    }

    protected override void OnSlotLeftButtonDoubleClicked(int number)
    {
        _connection.WriteAndFlush(ClickInventoryInput.LeftDoubleClick(number));
    }

    protected override void OnSlotRightMouseButtonReleased(int number)
    {
        Log.Debug("Right released on {}", number);
    }

    public void OnShortcutButtonClicked(Connection connection)
    {
        if (Visible)
        {
            Visible = false;
            return;
        }
        connection.WriteAndFlush(SimpleInput.Inventory);
    }

    private void SetSlot(InventoryItemMessage item)
    {
        GetSlot(item.Slot).SetTextureAndTip(_icons[item.Icon], item.ToolTip, item.Color);
    }

    public void UpdateSlot(InventoryItemMessage message)
    {
        if (!Visible)
            return;
        var slot = GetSlot(message.Slot);
        if (message.Removed)
            slot.Clear();
        else
        {
            _message.ReplaceOrAdd(message);
            SetSlot(message);
        }
    }


    public void StartDropItem(ItemModifyInput window, string name, int number, int slot, Vector2I coordinate)
    {
        window.SetInUse(true);
        window.SetExtra("coordinate", coordinate);
        window.SetExtra("slot", slot);
        window.Confirmed = OnDropItemConfirmed;
        window.SetNameAndNumber(name, number);
    }
    
    private void OnDropItemConfirmed(ItemModifyInput input)
    {
        _connection.WriteAndFlush(new ConfirmDropItemInput(input.GetExtra<int>("slot"), input.Number, input.GetExtra<Vector2I>("coordinate")));
        input.SetInUse(false);
    }


    public void UpdateInventoryView(InventoryMessage message, Connection connection)
    {
        if (!message.Forceful && !Visible)
            return;
        _connection = connection;
        _message = message;
        ForeachSlot(sl => sl.Clear());
        foreach (var item in message.Items)
        {
            SetSlot(item);
        }
        Visible = true;
    }
}