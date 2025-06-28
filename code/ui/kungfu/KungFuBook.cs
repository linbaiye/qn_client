using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using NLog;
using QnClient.code.input;
using QnClient.code.message;
using QnClient.code.network;
using QnClient.code.sprite;
using ILogger = NLog.ILogger;

namespace QnClient.code.ui.kungfu;

public partial class KungFuBook : AbstractSlotContainer
{
    
    private readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    private readonly ZipFileSpriteLoader _zipFileSpriteLoader = ZipFileSpriteLoader.Instance;

    private Texture2D[] _icons;


    private KungFuTab _unnamedTab;
    private KungFuTab _basicTab;
    
    private KungFuBookMessage _message;
    
    private Connection _connection;

    public override void _Ready()
    {
        base._Ready();
        Visible = false;
        _icons = _zipFileSpriteLoader.LoadOrderedMagicIcons();
        _unnamedTab = GetNode<KungFuTab>("UnnamedTab");
        _unnamedTab.SetTextures(ResourceLoader.Load<Texture2D>("res://ui/kungfu/unnamed.png"), ResourceLoader.Load<Texture2D>("res://ui/kungfu/unnamed_focus.png"));
        _unnamedTab.Pressed += OnUnnamedPressed;
        _basicTab = GetNode<KungFuTab>("BasicTab");
        _basicTab.SetTextures(ResourceLoader.Load<Texture2D>("res://ui/kungfu/basic.png"), ResourceLoader.Load<Texture2D>("res://ui/kungfu/basic_focus.png"));
        _basicTab.Pressed += OnBasicPressed;
    }

    protected override Slot CreateSlot(string name)
    {
        return Slot.Create(name, new Vector2(28, 28), new Vector2(28, 28));
    }

    protected override void OnSlotLeftMouseButtonReleased(int number)
    {
        Logger.Debug("Slot {} released.", number);
        var slot = FindSlotHasHovering();
        if (slot != null && slot.Number != number)
        {
            Logger.Debug("Slot {} has hovering.", slot.Number);
            // swap kungfu
        }
        else
        {
            HandleKungFuClick(number, (i, fu) => _connection.WriteAndFlush(ClickKungFuInput.LeftClick(i, fu.Slot)));
        }
    }


    /// <summary>
    /// When the '武功' button clicked.
    /// </summary>
    /// <param name="connection"></param>
    public void OnShortcutButtonClicked(Connection connection)
    {
        if (Visible)
        {
            Visible = false;
            return;
        }
        connection.WriteAndFlush(SimpleInput.KungFuBook);
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
        ForeachSlot(slot => slot.SetTextureAndTip(_icons[0], ""));
        foreach (var kungFu in kungFuList)
        {
            GetSlot(kungFu.Slot).SetTextureAndTip(_icons[kungFu.Icon], kungFu.ToolTip);
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


    private void RefreshFocusedTab()
    {
        RefreshKungFuSlots(_unnamedTab.IsFocused ? _message.Unnamed : _message.Basic);
    }

    public void ShowKungFuBook(KungFuBookMessage message, Connection connection)
    {
        _message = message;
        _connection = connection;
        if (!_unnamedTab.IsFocused && !_basicTab.IsFocused)
            _unnamedTab.GainFocus();
        RefreshFocusedTab();
        Visible = true;
    }

}