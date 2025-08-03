using System.Collections.Generic;
using System.Text.Json;
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
    private readonly Texture2D[] _kungFuIcons;
    
    private ulong _lastKeyPressedMillis = 0;

    
    private FileStorage? _file;
    
    private static readonly Dictionary<Key, int> KeySlotMap = new()
    {
        { Key.Q, 5 },
        { Key.W, 6 },
        { Key.E, 7 },
        { Key.R, 8 },
        { Key.Key1, 1 },
        { Key.Key2, 2 },
        { Key.Key3, 3 },
        { Key.Key4, 4 },
    };

    private static readonly string[] CodeMap = ["Q", "W", "E", "R"];

    
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
        _kungFuIcons = ZipFileSpriteLoader.Instance.LoadOrderedMagicIcons();
        _inventory.KeyPressedOnSlot += OnInventoryKeyPressedOnSlot;
        _inventory.HotKeySlotUpdated += OnInventoryHotKeySlotUpdated;
        _kungFuBook.KeyPressedOnSlot += OnKungFuKeyPressedOnSlot;
        _kungFuBook.HotKeySlotUpdated += OnKungFuHotKeySlotUpdated;
        for (int i = 1; i <= 8; i++)
        {
            var node = Slot.Create("Slot" + i, new Vector2(28, 28), new Vector2(28, 28), false);
            node.LeftMouseButtonDoubleClicked += Clicked;
            node.LeftMouseButtonReleased += Clicked;
            if (i <= 4)
            {
                _leftHotKeys.AddChild(node);
                node.SetKeyLabel(i.ToString());
            }
            else
            {
                _rightHotKeys.AddChild(node);
                node.SetKeyLabel(CodeMap[i-5]);
            }
        }
    }

    private void Clicked(int n)
    {
        _inventory.HotKeyPressed(n);
        _kungFuBook.HotKeyPressed(n);
    }

    public void OnCharacterJoined()
    {
        _file = new FileStorage("hotkeys");
        RestoreHotKeys();
        _inventory.SyncQuietly();
        _kungFuBook.SyncQuietly();
    }

    private void RestoreHotKeys()
    {
        var content = _file.ReadContent();
        if (string.IsNullOrEmpty(content))
        {
            return;
        }
        try
        {
            var ret = JsonSerializer.Deserialize<Dictionary<string, string>>(content);
            if (ret.TryGetValue("inventory", out var inventoryKeys))
            {
                _inventory.BindHotKeys(inventoryKeys);
            }

            if (ret.TryGetValue("kungfu", out var kungfuKeys))
            {
                _kungFuBook.BindHotKeys(kungfuKeys);
            }
        }
        catch
        {
            _file.Delete();
        }
    }



    public bool HandleKeyEvent(InputEvent @event)
    {
        if (@event is not InputEventKey key)
        {
            return false;
        }
        if (!KeySlotMap.TryGetValue(key.Keycode, out var hotkeySlotNumber))
            return false;
        var current = Time.GetTicksMsec();
        if (_lastKeyPressedMillis + 200 > current || key.IsReleased())
            return true;
        _lastKeyPressedMillis = current;
        _inventory.HotKeyPressed(hotkeySlotNumber);
        _kungFuBook.HotKeyPressed(hotkeySlotNumber);
        return true;
    }


    private Slot GetSlot(int slotNumber)
    {
        if (slotNumber > 4)
            return _rightHotKeys.GetNode<Slot>("Slot" + slotNumber);
        return _leftHotKeys.GetNode<Slot>("Slot" + slotNumber);
    }

    private void OnKungFuKeyPressedOnSlot(int slotNumber, InputEventKey key)
    {
        OnKeyPressedOnSlot(slotNumber, key, false);
    }

    private void OnKungFuHotKeySlotUpdated(bool removed, int hotKeySlotNumber, KungFuBookMessage.KungFu? kungFu)
    {
        var hotKey = GetSlot(hotKeySlotNumber);
        if (removed)
        {
            hotKey.ClearTextureAndTip();
        }
        else if (kungFu != null)
        {
            hotKey.SetDetails(_kungFuIcons[kungFu.Icon], kungFu.FormatKungFuTip());
        }
    }

    private void OnInventoryHotKeySlotUpdated(bool removed, int hotKeySlotNumber, InventoryItemMessage? message)
    {
        var hotKey = GetSlot(hotKeySlotNumber);
        if (removed)
        {
            hotKey.ClearTextureAndTip();
        }
        else if (message != null)
        {
            hotKey.SetDetails(_itemIcons[message.Value.Icon], message.Value.Tip, message.Value.Color);
        }
    }


    private void OnKeyPressedOnSlot(int slotNumber, InputEventKey key, bool inventory)
    {
        if (!KeySlotMap.ContainsKey(key.Keycode))
            return;
        var slot = inventory ? _inventory.GetSlot(slotNumber) : _kungFuBook.GetSlot(slotNumber);
        var container = key.Keycode >= Key.Key1 && key.Keycode <= Key.Key4 ? _leftHotKeys : _rightHotKeys;
        if (KeySlotMap.TryGetValue(key.Keycode, out var hotkeySlotNumber))
        {
            var hotKeySlot = container.GetNode<Slot>("Slot" + hotkeySlotNumber);
            hotKeySlot.CopyDetails(slot);
            if (inventory)
            {
                _kungFuBook.RemoveHotKey(hotkeySlotNumber);
                _inventory.BindHotKey(hotkeySlotNumber, slotNumber);
            }
            else
            {
                _inventory.RemoveHotKey(hotkeySlotNumber);
                _kungFuBook.BindHotkey(hotkeySlotNumber, slotNumber);
            }
        }
    }

    public void Save()
    {
        if (_file == null)
            return;
        Dictionary<string, string> hotKeys = new Dictionary<string, string>();
        hotKeys.Add("inventory", _inventory.SerializeHotKeys);
        hotKeys.Add("kungfu", _kungFuBook.SerializeHotKeys);
        var serialize = JsonSerializer.Serialize(hotKeys);
        _file.Save(serialize);
    }

    private void OnInventoryKeyPressedOnSlot(int slotNumber, InputEventKey key)
    {
        OnKeyPressedOnSlot(slotNumber, key, true);
    }
}