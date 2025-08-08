using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
    public event Action<string, int>? ItemDragReleased;

    // Removed?, HotKeySlotNumber, 
    public event Action<bool, int, InventoryItemMessage?>? HotKeySlotUpdated;
    

    private ItemModifyInput _input;

    private class DoubleClickHandler(object o, Action<int, string, long> a)
    {
        public object Owner { get;  } = o;
        // SlotNumber, ItemName, ItemNumber
        public Action<int, string, long> Action { get; } = a;
    }

    private readonly List<DoubleClickHandler> _handlers = new();
    
    public override void _Ready()
    {
        base._Ready();
        _icons = _zipFileSpriteLoader.LoadOrderedItemIcons();
        _input = ItemModifyInput.Create();
        var p = new Vector2(-_input.GetSize().X, GetSize().Y - _input.GetSize().Y);
        _input.Position = p;
        AddChild(_input);
    }

    public void InstallDoubleClickHandler(object owner, Action<int, string, long> handler)
    {
        foreach (var h in _handlers)
        {
            if (Equals(owner, h.Owner))
            {
                return;
            }
        }
        _handlers.Add(new DoubleClickHandler(owner, handler));
    }
    
    public void UninstallDoubleClickHandler(object owner)
    {
        foreach (var h in _handlers)
        {
            if (Equals(owner, h.Owner))
            {
                _handlers.Remove(h);
                break;
            }
        }
    }

    protected override Slot CreateSlot(string name)
    {
        return Slot.Create(name, new Vector2(40, 40), new Vector2(30, 30), false);
    }

    protected override void OnDragReleased(int number)
    {
        var slot = FindSlotHasHovering();
        if (slot != null && slot.SlotNumber != number)
        {
            _connection.WriteAndFlush(SwapSlotInput.Inventory(number, slot.SlotNumber));
            return;
        }
        foreach (var messageItem in _message.Items)
        {
            if (messageItem.Slot == number)
            {
                ItemDragReleased?.Invoke(messageItem.Name, number);
                break;
            }
        }
    }

    private string GetItemName(int slot)
    {
        foreach (var item in _message.Items)
        {
            if (item.Slot == slot)
                return item.Name;
        }
        return null;
    }
    
    private long GetItemNumber(int slot)
    {
        foreach (var item in _message.Items)
        {
            if (item.Slot == slot)
                return item.Number;
        }
        return 0;
    }

    protected override void OnSlotLeftButtonDoubleClicked(int number)
    {
        if (_handlers.Count != 0)
            _handlers.Last().Action(number, GetItemName(number), GetItemNumber(number));
        else
            _connection.WriteAndFlush(ClickInventoryInput.LeftDoubleClick(number));
    }

    protected override void OnSlotRightMouseButtonReleased(int number)
    {
        _connection.WriteAndFlush(ClickInventoryInput.RightClick(number));
    }

    protected override int Capacity => 30;

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
        GetSlot(item.Slot).SetDetails(_icons[item.Icon], item.Tip, item.Color);
    }


    private void NotifyHotKey(InventoryItemMessage message)
    {
        var hotKeySlot = GetHotKeySlot(message.Slot);
        if (hotKeySlot == 0)
            return;
        HotKeySlotUpdated?.Invoke(message.Removed, hotKeySlot, message);
    }

    public void UpdateSlot(InventoryItemMessage message)
    {
        NotifyHotKey(message);
        if (!Visible)
            return;
        var slot = GetSlot(message.Slot);
        if (message.Removed)
            slot.ClearTextureAndTip();
        else
        {
            _message.ReplaceOrAdd(message);
            SetSlot(message);
        }
    }


    public void StartDropItem(string name, int number, int slot, Vector2I coordinate)
    {
        _input.SetInUse(true);
        _input.SetExtra("coordinate", coordinate);
        _input.SetExtra("slot", slot);
        _input.Confirmed = OnDropItemConfirmed;
        _input.SetNameNumber(name, number);
    }
    
    private void OnDropItemConfirmed(ItemModifyInput input)
    {
        _connection.WriteAndFlush(new ConfirmDropItemInput(input.GetExtra<int>("slot"), input.Number, input.GetExtra<Vector2I>("coordinate")));
        input.SetInUse(false);
    }


    private void NotifyHotKey(InventoryMessage message)
    {
        foreach (var keyValuePair in _hotKeys)
        {
            bool found = false;
            foreach (var inventoryItemMessage in message.Items)
            {
                if (inventoryItemMessage.Slot == keyValuePair.Value)
                {
                    HotKeySlotUpdated?.Invoke(false, keyValuePair.Key, inventoryItemMessage);
                    found = true;
                    break;
                }
            }
            if (!found)
                HotKeySlotUpdated?.Invoke(true, keyValuePair.Key, null);
        }
    }
    
    public void UpdateInventoryView(InventoryMessage message)
    {
        _message = message;
        NotifyHotKey(message);
        if (!message.Forceful && !Visible)
            return;
        ForeachSlot(sl => sl.ClearTextureAndTip());
        foreach (var item in message.Items)
        {
            SetSlot(item);
        }

        Visible = true;
    }

    public void SetConnection(Connection connection)
    {
        _connection = connection;
    }
    
    private Dictionary<int, int> _hotKeys = new();

    public void BindHotKeys(string keys)
    {
        _hotKeys = JsonSerializer.Deserialize<Dictionary<int, int>>(keys);
    }

    public void BindHotKey(int source, int slotNumber)
    {
        _hotKeys.Remove(source);
        _hotKeys.Add(source, slotNumber);
    }

    private int GetHotKeySlot(int inventorySlotNumber)
    {
        foreach (var keyValuePair in _hotKeys)
        {
            if (keyValuePair.Value == inventorySlotNumber)
                return keyValuePair.Key;
        }
        return 0;
    }

    public void RemoveHotKey(int source)
    {
        _hotKeys.Remove(source);
    }
    
    public string SerializeHotKeys => JsonSerializer.Serialize(_hotKeys);

    public void HotKeyPressed(int source)
    {
        if (_hotKeys.TryGetValue(source, out var number))
        {
            _connection.WriteAndFlush(ClickInventoryInput.LeftDoubleClick(number));
        }
    }

    public void SyncQuietly()
    {
        _connection.WriteAndFlush(SimpleInput.InventoryQuietly);
    }

}