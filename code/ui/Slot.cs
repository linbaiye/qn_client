using System;
using System.Text.RegularExpressions;
using Godot;

namespace QnClient.code.ui;

public partial class Slot : CenterContainer
{
    private SlotButton _slotButton;

    public event Action<int>? LeftMouseButtonReleased;
    public event Action<int>? RightMouseButtonReleased;
    public event Action<int>? LeftMouseButtonDoubleClicked;

    private Vector2 _slotSize;
    private Vector2 _iconSize;
    private bool _scaleTexture;
    // private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
    
    public override void _Ready()
    {
        _slotButton = GetNode<SlotButton>("SlotButton");
        _slotButton.RightButtonReleased += OnRightButtonReleased;
        _slotButton.LeftButtonDoubleClicked += OnLeftButtonDoubleClicked;
        _slotButton.LeftButtonReleased += OnLeftButtonReleased;
        Number = GetNumber();
        _slotButton.CustomMinimumSize = _iconSize;
        _slotButton.StretchMode =
            _scaleTexture ? TextureButton.StretchModeEnum.Scale : TextureButton.StretchModeEnum.KeepCentered;
        CustomMinimumSize = _slotSize;
        Size = _slotSize;
    }
    
    public int Number { get; private set; }

    public bool MouseHovering => _slotButton.MouseHovering;

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
    
    private void OnLeftButtonDoubleClicked()
    {
        if (Number != -1)
        {
            LeftMouseButtonDoubleClicked?.Invoke(Number);
        }
    }

    public void SetTextureAndTip(Texture2D texture2D, string tip, int color = 0)
    {
        _slotButton.TextureNormal = texture2D;
        _slotButton.TooltipText = tip;
        if (color != 0)
            _slotButton.Material = DyeShader.CreateShaderMaterial(color);
    }

    public void Clear()
    {
        _slotButton.TextureNormal = null;
        _slotButton.TooltipText = null;
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