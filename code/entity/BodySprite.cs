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
    
    public override void _Ready()
    {
        _mouseArea = GetNode<MouseArea>("MouseArea");
        _mouseArea.MouseEntered += () => MouseEntered?.Invoke();
        _mouseArea.MouseExited += () => MouseExited?.Invoke();
        _mouseArea.Clicked += () => Clicked?.Invoke();
        _mouseArea.AttackInvoked += () => AttackInvoked?.Invoke();
    }
    
    public MouseArea MouseArea => _mouseArea;
}