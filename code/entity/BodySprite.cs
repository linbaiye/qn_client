using System;
using Godot;
using QnClient.code.hud;

namespace QnClient.code.entity;

public partial class BodySprite: Sprite2D
{
    private MouseArea _mouseArea;

    public event Action? MouseEntered;
    
    public event Action? MouseExited;
    
    public event Action? AttackInvoked;
    
    public event Action? Clicked;

    private bool _hovering;
    
    public override void _Ready()
    {
        _mouseArea = GetNode<MouseArea>("MouseArea");
        _mouseArea.MouseEntered += OnMouseEntered;
        _mouseArea.MouseExited += OnMouseExited;
        _mouseArea.Clicked += () => Clicked?.Invoke();
        _mouseArea.AttackInvoked += () => AttackInvoked?.Invoke();
    }

    private void OnMouseEntered()
    {
        _hovering = true;
        MouseEntered?.Invoke();
    }
    
    private void OnMouseExited()
    {
        _hovering = false;
        MouseExited?.Invoke();
    }
    
    public MouseArea MouseArea => _mouseArea;

    public bool IsCovering(Vector2 position)
    {
        return _hovering;
        /*var start = _mouseArea.Position;
        var end = start + _mouseArea.GetSize();
        return start.X <= position.X && end.X >= position.X &&
               start.Y <= position.Y && end.Y >= position.Y;*/
    }
}