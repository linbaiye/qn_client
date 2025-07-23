using System;
using Godot;

namespace QnClient.code.hud;

public partial class MouseArea : Panel
{
    public event Action? AttackInvoked;
    
    public event Action? Clicked;

    private MouseEventHandler _mouseEventHandler;

    public override void _Ready()
    {
        _mouseEventHandler = GetNode<MouseEventHandler>("MouseEventHandler");
        _mouseEventHandler.Clicked += () => Clicked?.Invoke();
        _mouseEventHandler.DoubleClicked += () => AttackInvoked?.Invoke();
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton mouseButton || mouseButton.ButtonIndex != MouseButton.Left)
        {
            return;
        }
        GetViewport().SetInputAsHandled();
        _mouseEventHandler.HandleMouseButton(mouseButton);
    }
}