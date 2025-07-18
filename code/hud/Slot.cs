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

    private Vector2 _slotSize;
    private Vector2 _iconSize;
    private bool _scaleTexture;
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
    
    public bool MouseHovering { get; private set; }
    private const float DoubleClickThreshold = 0.2f;
    private float _lastClickTime;
    private Timer _timer;
    
    public override void _Ready()
    {
        _slotTexture = GetNode<TextureRect>("SlotTexture");
        Number = GetNumber();
        _slotTexture.CustomMinimumSize = _iconSize;
        _slotTexture.StretchMode =
            _scaleTexture ? TextureRect.StretchModeEnum.Scale : TextureRect.StretchModeEnum.KeepCentered;
        CustomMinimumSize = _slotSize;
        Size = _slotSize;
        MouseEntered += () => MouseHovering = true;
        MouseExited += () => MouseHovering = false;
        _timer = GetNode<Timer>("Timer");
        _timer.OneShot = true;
        _timer.Timeout += OnLeftButtonReleased;
    }
    
    public int Number { get; private set; }


    private int GetNumber()
    {
        var match = Regex.Match(GetName(), "(\\d+)");
        return !match.Success ? -1 : int.Parse(match.Groups[1].Value);
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


    public override void _GuiInput(InputEvent @event)
    {
        if (@event is not InputEventMouse mouse)
        {
            return;
        }
        GetViewport().SetInputAsHandled();
        if (mouse is not InputEventMouseButton button)
        {
            return;
        }
        if (button.ButtonIndex == MouseButton.Right && button.IsReleased())
        {
            OnRightButtonReleased();
            return;
        }
        if (button.ButtonIndex != MouseButton.Left)
        {
            return;
        }
        if (button.IsReleased())
        {
            var cur = Time.GetTicksMsec() / 1000;
            if (cur - _lastClickTime <= DoubleClickThreshold)
            {
                OnLeftButtonDoubleClicked();
                _timer.Stop();
            }
            else
            {
                _timer.Start(DoubleClickThreshold);
            }
            _lastClickTime = cur;
        }
    }

    private void OnLeftButtonDoubleClicked()
    {
        if (Number != -1)
        {
            LeftMouseButtonDoubleClicked?.Invoke(Number);
        }
    }

    public void SetTextureAndTip(Texture2D texture2D, string tip, int color = 0)
    {
        _slotTexture.Texture = texture2D;
        if (color != 0)
            _slotTexture.Material = DyeShader.CreateShaderMaterial(color);
        TooltipText = tip;
    }

    private string Tip => TooltipText;

    public void Clear()
    {
        _slotTexture.Texture = null;
        TooltipText = null;
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