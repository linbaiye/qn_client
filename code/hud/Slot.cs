using System;
using System.Text.RegularExpressions;
using Godot;
using NLog;
using QnClient.code.entity;

namespace QnClient.code.hud;

public partial class Slot : Panel
{
    private TextureRect _slotTexture;
    public event Action<int>? LeftMouseButtonReleased;
    public event Action<int>? RightMouseButtonReleased;
    public event Action<int>? LeftMouseButtonDoubleClicked;
    public event Action<int, InputEventKey>? KeyPressed;

    private Vector2 _slotSize;
    private Vector2 _iconSize;
    private bool _scaleTexture;
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
    public bool MouseHovering { get; private set; }
    
    private Label _keyLabel;
    
    private MouseEventHandler _mouseEventHandler;

    private Label _attributeTip;
    
    private string _tipText;
    
    public override void _Ready()
    {
        _slotTexture = GetNode<TextureRect>("SlotTexture");
        _slotTexture.CustomMinimumSize = _iconSize;
        _slotTexture.StretchMode =
            _scaleTexture ? TextureRect.StretchModeEnum.Scale : TextureRect.StretchModeEnum.KeepCentered;
        CustomMinimumSize = _slotSize;
        Size = _slotSize;
        _mouseEventHandler = GetNode<MouseEventHandler>("MouseEventHandler");
        MouseEntered += () =>
        {
            MouseHovering = true;
        };
        MouseExited += OnMouseExited;
        _mouseEventHandler.DoubleClicked += OnLeftButtonDoubleClicked;
        _mouseEventHandler.RightClicked += OnRightButtonReleased;
        _mouseEventHandler.Clicked += OnLeftButtonReleased;
        _keyLabel = GetNode<Label>("KeyLabel");
        _attributeTip = GetNode<Label>("AttributeTip");
        _attributeTip.Visible = false;
        Number = GetNumber();
    }
    
    public int Number { get; private set; }

    private int GetNumber()
    {
        var match = Regex.Match(GetName(), "(\\d+)");
        return !match.Success ? -1 : int.Parse(match.Groups[1].Value);
    }

    private void OnMouseExited()
    {
        MouseHovering = false;
        _attributeTip.Visible = false;
        TooltipText = _tipText;
    }
    

    private void OnLeftButtonReleased()
    {
        if (Number != -1)
        {
            LeftMouseButtonReleased?.Invoke(Number);
        }
    }

    private void OnRightButtonReleased()
    {
        if (Number != -1)
        {
            RightMouseButtonReleased?.Invoke(Number);
        }
    }


    public void ShowAttributeTipIfHasHover(string text)
    {
        if (!MouseHovering)
            return;
        TooltipText = null;
        _attributeTip.Text = text;
        _attributeTip.Visible = true;
        var globalMousePosition = GetGlobalMousePosition();
        var localMousePosition = GetLocalMousePosition();
        var textSize = _attributeTip.Size;
        var windowSize = GetViewport().GetWindow().Size;
        float x = localMousePosition.X;
        if (globalMousePosition.X + textSize.X > windowSize.X)
        {
            x = -textSize.X;
        }
        _attributeTip.Position = new Vector2(x, localMousePosition.Y);
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is not InputEventKey key || !MouseHovering)
        {
            return;
        }
        GetViewport().SetInputAsHandled();
        if (key.IsPressed() && Number != -1)
            KeyPressed?.Invoke(Number, key);
    }

    public void SetKeyLabel(string key)
    {
        _keyLabel.Text = key;
    }


    public override void _GuiInput(InputEvent @event)
    {
        if (@event is not InputEventMouse mouse)
        {
            return;
        }
        GetViewport().SetInputAsHandled();
        if (mouse is InputEventMouseButton button)
        {
            _mouseEventHandler.HandleMouseButton(button);
        }
    }

    private void OnLeftButtonDoubleClicked()
    {
        if (Number != -1)
        {
            LeftMouseButtonDoubleClicked?.Invoke(Number);
        }
    }

    public void SetDetails(Texture2D texture2D, string tip, int color = 0)
    {
        _slotTexture.Texture = texture2D;
        if (color != 0)
            _slotTexture.Material = DyeShader.CreateShaderMaterial(color);
        TooltipText = tip;
        _tipText = tip;
    }
    
    public bool Empty => _slotTexture.Texture == null;

    public void ClearTextureAndTip()
    {
        _slotTexture.Texture = null;
        TooltipText = null;
        _tipText = null;
    }

    public void CopyDetails(Slot another)
    {
        _slotTexture.Texture = another._slotTexture.Texture;
        _slotTexture.Material = another._slotTexture.Material;
        TooltipText = another.TooltipText;
        _tipText = another._tipText;
    }


    public static Slot Create(string name, Vector2 slotSize, Vector2 iconSize, bool scale = true)
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://scene/ui/slot.tscn");
        var slot = scene.Instantiate<Slot>();
        slot.SetName(name);
        slot._iconSize = iconSize;
        slot._slotSize = slotSize;
        slot._scaleTexture = scale;
        return slot;
    }
}