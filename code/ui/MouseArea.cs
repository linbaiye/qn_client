using System;
using Godot;

namespace QnClient.code.ui;

public partial class MouseArea : Panel
{

    public event Action? AttackInvoked;
    
    public event Action? Clicked;
    
    public override void _GuiInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton mouseButton || mouseButton.ButtonIndex != MouseButton.Left)
        {
            return;
        }
        if (mouseButton.IsDoubleClick())
        {
            GD.Print("Double click with key? " + Input.IsKeyPressed(Key.Shift));
            AttackInvoked?.Invoke();
        }
        else if (mouseButton.Pressed)
        {
            Clicked?.Invoke();
        }
        GetViewport().SetInputAsHandled();
    }
}