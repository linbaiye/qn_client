using System.Collections.Generic;
using Godot;
using QnClient.code.hud.inventory;
using QnClient.code.hud.kungfu;
using QnClient.code.message;
using QnClient.code.sprite;

namespace QnClient.code.hud.hotkey;

public class HotKeyManager
{
    
    private readonly HBoxContainer _leftHotKeys;
    
    private readonly HBoxContainer _rightHotKeys;

    private readonly Inventory _inventory;
    private readonly KungFuBook _kungFuBook;
    private readonly Texture2D[] _itemIcons;
    
    private ulong _lastKeyPressedMillis = 0;
    
    public HotKeyManager(HBoxContainer leftHotKeys,
        HBoxContainer rightHotKeys,
        Inventory inventory,
        KungFuBook kungFuBook)
    {
        _leftHotKeys = leftHotKeys;
        _rightHotKeys = rightHotKeys;
        _inventory = inventory;
        _kungFuBook = kungFuBook;
        _itemIcons = ZipFileSpriteLoader.Instance.LoadOrderedItemIcons();
        _inventory.SlotKeyPressed += OnInventorySlotKeyPressed;
    }


    public bool HandleKeyEvent(InputEvent @event)
    {
        if (@event is not InputEventKey key)
        {
            return false;
        }
        if (key.Keycode < Key.Key1 || key.Keycode > Key.Key4)
            return false;
        var current = Time.GetTicksMsec();
        if (_lastKeyPressedMillis + 200 > current || key.IsReleased())
            return true;
        _lastKeyPressedMillis = current;
        var hotKeySlot = _leftHotKeys.GetNode<Slot>("Slot" + ((int)key.Keycode - (int)Key.Key0));
        _inventory.HotKeyPressed(hotKeySlot);
        return true;
    }


    public void OnKungFuSlotKeyPressed(int slotNumber, InputEventKey key)
    {
        
    }

    public void OnInventorySlotUpdated(InventoryItemMessage message)
    {
        var hotKey = (Slot)_inventory.GetHotKey(message.Slot);
        if (hotKey == null)
            return;
        if (message.Removed)
        {
            hotKey.Clear();
            _inventory.RemoveHotKey(hotKey);
        }
        else
        {
            hotKey.SetDetails(_itemIcons[message.Icon], message.Tip, message.Color);
        }
    }

    public void OnInventoryUpdated(InventoryMessage message)
    {
        foreach (var item in message.Items)
        {
            OnInventorySlotUpdated(item);
        }
    }

    private static readonly Dictionary<Key, int> RightKeyMap = new()
    {
        { Key.Q, 1 },
        { Key.W, 2 },
        { Key.E, 3 },
        { Key.R, 4 },
        { Key.Key1, 1 },
        { Key.Key2, 2 },
        { Key.Key3, 3 },
        { Key.Key4, 4 },
    };

    private void OnSlotKeyPressed(int slotNumber, InputEventKey key, bool inventory)
    {
        if (key.Keycode < Key.Key1 || key.Keycode > Key.Key4 || !RightKeyMap.ContainsKey(key.Keycode))
            return;
        var slot = inventory ? _inventory.GetSlot(slotNumber) : _kungFuBook.GetSlot(slotNumber);
        var container = key.Keycode >= Key.Key1 && key.Keycode <= Key.Key4 ? _leftHotKeys : _rightHotKeys;
        if (RightKeyMap.TryGetValue(key.Keycode, out var hotkeySlotNumber))
        {
            var hotKeySlot = container.GetNode<Slot>("Slot" + hotkeySlotNumber);
            hotKeySlot.CopyDetails(slot);
            hotKeySlot.SetKeyLabel(key.AsText());
            if (inventory)
            {
                _kungFuBook
                _inventory.BindHotKey(hotKeySlot, slotNumber);
            }
        }
    }

    private void OnInventorySlotKeyPressed(int slotNumber, InputEventKey key)
    {
        OnSlotKeyPressed(slotNumber, key, true);
    }
}