using System;
using System.Collections.Generic;
using System.Linq;
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
    

    private ItemModifyInput _input;

    private class DoubleClickHandler(object o, Action<int, string, long> a)
    {
        public object Owner { get;  } = o;
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
        GetSlot(item.Slot).SetDetails(_icons[item.Icon], item.Tip, item.Color);
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

    public void SetConnection(Connection connection)
    {
        _connection = connection;
    }
    
    private readonly Dictionary<object, int> _hotKeys = new();

    public void BindHotKey(object source, int slotNumber)
    {
        _hotKeys.Remove(source);
        _hotKeys.Add(source, slotNumber);
    }

    public object GetHotKey(int slotNumber)
    {
        foreach (var keyValuePair in _hotKeys)
        {
            if (keyValuePair.Value == slotNumber)
                return keyValuePair.Key;
        }
        return default;
    }

    public void RemoveHotKey(object source)
    {
        _hotKeys.Remove(source);
    }

    public void HotKeyPressed(object source)
    {
        if (_hotKeys.TryGetValue(source, out var number))
        {
            _connection.WriteAndFlush(ClickInventoryInput.LeftDoubleClick(number));
        }
    }
}