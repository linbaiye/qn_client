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
using ILogger = NLog.ILogger;

namespace QnClient.code.hud.kungfu;

public partial class KungFuBook : AbstractSlotContainer, IConnectionAware
{
    
    private readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    private readonly ZipFileSpriteLoader _zipFileSpriteLoader = ZipFileSpriteLoader.Instance;

    private Texture2D[] _icons;


    private hud.Tab _unnamedTab;
    private hud.Tab _basicTab;
    public event Action<bool, int, KungFuBookMessage.KungFu?>? HotKeySlotUpdated;
    
    private KungFuBookMessage _message;
    
    private Connection _connection;

    public override void _Ready()
    {
        base._Ready();
        Visible = false;
        _icons = _zipFileSpriteLoader.LoadOrderedMagicIcons();
        _unnamedTab = GetNode<hud.Tab>("UnnamedTab");
        _unnamedTab.SetTextures(ResourceLoader.Load<Texture2D>("res://ui/kungfu/unnamed.png"), ResourceLoader.Load<Texture2D>("res://ui/kungfu/unnamed_focus.png"));
        _unnamedTab.Pressed += OnUnnamedPressed;
        _basicTab = GetNode<hud.Tab>("BasicTab");
        _basicTab.SetTextures(ResourceLoader.Load<Texture2D>("res://ui/kungfu/basic.png"), ResourceLoader.Load<Texture2D>("res://ui/kungfu/basic_focus.png"));
        _basicTab.Pressed += OnBasicPressed;
    }


    public void SetConnection(Connection connection)
    {
        _connection = connection;
    }

    protected override Slot CreateSlot(string name)
    {
        return Slot.Create(name, new Vector2(28, 28), new Vector2(28, 28));
    }

    protected override void OnSlotLeftMouseButtonReleased(int number)
    {
        var slot = FindSlotHasHovering();
        if (slot != null && slot.Number != number)
        {
            if (!_basicTab.IsFocused)
                return;
            _connection.WriteAndFlush(SwapSlotInput.KungFu(2, number, slot.Number));
        }
        else
        {
            HandleKungFuClick(number, (i, fu) => _connection.WriteAndFlush(ClickKungFuInput.LeftClick(i, fu.Slot)));
        }
    }


    /// <summary>
    /// When the '武功' button clicked.
    /// </summary>
    public void OnShortcutButtonClicked()
    {
        if (Visible)
        {
            Visible = false;
            return;
        }
        _connection.WriteAndFlush(SimpleInput.KungFuBook);
    }


    private void HandleKungFuClick(int slotNumber, Action<int, KungFuBookMessage.KungFu> actionWhenFound)
    {
        if (_unnamedTab.IsFocused)
        {
            foreach (var kungFu in _message.Unnamed.Where(kungFu => kungFu.Slot == slotNumber))
            {
                actionWhenFound.Invoke(1, kungFu);
                break;
            }
        }
        if (_basicTab.IsFocused)
        {
            foreach (var kungFu in _message.Basic.Where(kungFu => kungFu.Slot == slotNumber))
            {
                actionWhenFound.Invoke(2, kungFu);
                break;
            }
        }
    }
    
    
    protected override void OnSlotLeftButtonDoubleClicked(int slotNumber)
    {
        HandleKungFuClick(slotNumber, (i, fu) => _connection.WriteAndFlush(ClickKungFuInput.LeftDoubleClick(i, fu.Slot)));
    }

    protected override void OnSlotRightMouseButtonReleased(int number)
    {
        Logger.Debug("Slot {} right rleased.", number);
    }

    private void RefreshKungFuSlots(List<KungFuBookMessage.KungFu> kungFuList)
    {
        ForeachSlot(slot => slot.SetDetails(_icons[0], ""));
        foreach (var kungFu in kungFuList)
        {
            GetSlot(kungFu.Slot).SetDetails(_icons[kungFu.Icon], kungFu.FormatKungFuTip());
        }
    }
    private void OnBasicPressed()
    {
        _basicTab.GainFocus();
        _unnamedTab.LoseFocus();
        RefreshKungFuSlots(_message.Basic);
    }
    

    private void OnUnnamedPressed()
    {
        _unnamedTab.GainFocus();
        _basicTab.LoseFocus();
        RefreshKungFuSlots(_message.Unnamed);
    }

    private void NotifyHotKey(KungFuBookMessage.KungFu kungFu, int page)
    {
        foreach (var keyValuePair in _hotkeyValues)
        {
            if (keyValuePair.Value.Page == page && kungFu.Slot == keyValuePair.Value.Slot)
            {
                HotKeySlotUpdated?.Invoke(false, keyValuePair.Key, kungFu);
                return;
            }
        }
    }

    public void KungFuGainExp(string name, int level)
    {
        foreach (var kungFu in _message.Basic)
        {
            if (kungFu.Name.Equals(name))
            {
                kungFu.Level = level;
                NotifyHotKey(kungFu, 2);
                if (_basicTab.IsFocused && Visible)
                {
                    RefreshFocusedTab();
                }
                return;
            }
        }
        foreach (var kungFu in _message.Unnamed)
        {
            if (kungFu.Name.Equals(name))
            {
                kungFu.Level = level;
                NotifyHotKey(kungFu, 1);
                break;
            }
        }
        if (Visible)
            RefreshFocusedTab();
    }


    private void RefreshFocusedTab()
    {
        RefreshKungFuSlots(_unnamedTab.IsFocused ? _message.Unnamed : _message.Basic);
    }
    
    private class HotkeyValue
    {
        private readonly int _page;
        private readonly int _slot;
        public HotkeyValue(int page, int slot)
        {
            _page = page;
            _slot = slot;
        }

        public int Page => _page;
        public int Slot => _slot;
    }
    
    private Dictionary<int, HotkeyValue> _hotkeyValues = new();

    public void RemoveHotKey(int source)
    {
        _hotkeyValues.Remove(source);
    }

    public void BindHotkey(int source, int slot)
    {
        RemoveHotKey(source);
        if (_unnamedTab.IsFocused)
        {
            _hotkeyValues.Add(source, new HotkeyValue(1, slot));
        }
        else
        {
            _hotkeyValues.Add(source, new HotkeyValue(2, slot));
        }
    }

    public string SerializeHotKeys => JsonSerializer.Serialize(_hotkeyValues);

    public void BindHotKeys(string keys)
    {
        _hotkeyValues = JsonSerializer.Deserialize<Dictionary<int, HotkeyValue>>(keys);
    }


    public void HotKeyPressed(int source)
    {
        if (_hotkeyValues.TryGetValue(source, out var hotkey))
        {
            _connection.WriteAndFlush(ClickKungFuInput.LeftDoubleClick(hotkey.Page, hotkey.Slot));
        }
    }

    private void NotifyHotKey(KungFuBookMessage message)
    {
        foreach (var keyValuePair in _hotkeyValues)
        {
            bool found = false;
            if (keyValuePair.Value.Page == 1)
            {
                foreach (var kungFu in message.Unnamed)
                {
                    if (kungFu.Slot == keyValuePair.Value.Slot)
                    {
                        found = true;
                        HotKeySlotUpdated?.Invoke(false, keyValuePair.Key, kungFu);
                        break;
                    }
                }
            }
            else
            {
                foreach (var kungFu in message.Basic)
                {
                    if (kungFu.Slot == keyValuePair.Value.Slot)
                    {
                        found = true;
                        HotKeySlotUpdated?.Invoke(false, keyValuePair.Key, kungFu);
                        break;
                    }
                }
            }
            if (!found)
                HotKeySlotUpdated?.Invoke(true, keyValuePair.Key, null);
        }
    }

    public void ShowKungFuBook(KungFuBookMessage message)
    {
        NotifyHotKey(message);
        _message = message;
        if (!message.Forcefull && !Visible)
            return;
        if (!_unnamedTab.IsFocused && !_basicTab.IsFocused)
            _unnamedTab.GainFocus();
        RefreshFocusedTab();
        Visible = true;
    }
    
    public void SyncQuietly()
    {
        _connection.WriteAndFlush(SimpleInput.KungFuBookQuietly);
    }
}