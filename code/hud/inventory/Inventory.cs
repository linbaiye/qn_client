using System;
using System.Collections.Generic;
using Godot;
using NLog;
using QnClient.code.input;
using QnClient.code.message;
using QnClient.code.network;
using QnClient.code.sprite;

namespace QnClient.code.hud.inventory;

public partial class Inventory : AbstractSlotContainer, IConnectionAware
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

    public void OnShortcutButtonClicked()
    {
        if (Visible)
        {
            Visible = false;
            return;
        }
        _connection.WriteAndFlush(SimpleInput.Inventory);
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
        window.SetNameNumber(name, number);
    }
    
    private void OnDropItemConfirmed(ItemModifyInput input)
    {
        _connection.WriteAndFlush(new ConfirmDropItemInput(input.GetExtra<int>("slot"), input.Number, input.GetExtra<Vector2I>("coordinate")));
        input.SetInUse(false);
    }
    
    public List<InventoryItemMessage> CloneItems()
    {
        List<InventoryItemMessage> result = new List<InventoryItemMessage>();
        if (_message == null)
            return result;
        foreach (var item in _message.Items)
        {
            result.Add(item.Clone());
        }
        return result;
    }

    public void UpdateInventoryView(InventoryMessage message)
    {
        _message = message;
        if (!message.Forceful && !Visible)
            return;
        ForeachSlot(sl => sl.Clear());
        foreach (var item in message.Items)
        {
            SetSlot(item);
        }
        Visible = true;
    }

    public bool CanAfford(int cost)
    {
        foreach (var message in _message.Items)
        {
            if (message.Name.Equals("钱币"))
                return message.Number >= cost;
        }
        return false;
    }

    public bool AddNonStack(string name, int icon, int color = 0)
    {
        if (_message.Items.Count >= 30)
            return false;
        int targetSlot = -1;
        for (int i = 1; i <= 30; i++)
        {
            targetSlot = i;
            foreach (var message in _message.Items)
            {
                if (message.Slot == i)
                {
                    targetSlot = -1;
                    break;
                }
            }
        }
        if (targetSlot == -1)
            return false;
        _message.Items.Add(new InventoryItemMessage(name, icon, targetSlot, 1, color));
        ForeachSlot(sl => sl.Clear());
        foreach (var item in _message.Items)
        {
            SetSlot(item);
        }
        return true;
    }

    public void Update(List<InventoryItemMessage> items)
    {
        if (_message == null)
            return;
        _message.Items = items;
        if (!Visible)
            return;
        ForeachSlot(sl => sl.Clear());
        foreach (var item in items)
        {
            SetSlot(item);
        }
    }

    public void SetConnection(Connection connection)
    {
        _connection = connection;
    }
}