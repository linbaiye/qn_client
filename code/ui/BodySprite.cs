using System;
using Godot;

namespace QnClient.code.ui;

public partial class BodySprite: Sprite2D
{
    private Panel _mouseArea;

    public event Action? MouseEntered;
    
    public event Action? MouseExited;
    
    public override void _Ready()
    {
        _mouseArea = GetNode<Panel>("MouseArea");
        _mouseArea.MouseEntered += MouseEnteredArea;
        _mouseArea.MouseExited += MouseExitedArea;
    }
    
    private void MouseEnteredArea()
    {
        MouseEntered?.Invoke();
    }
		
    private void MouseExitedArea()
    {
        MouseExited?.Invoke();
    }
}