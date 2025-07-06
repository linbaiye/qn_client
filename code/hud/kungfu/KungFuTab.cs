using System;
using Godot;

namespace QnClient.code.hud.kungfu;

public partial class KungFuTab : TextureRect
{

    private Texture2D _focusTexture;
    
    private Texture2D _normalTexture;

    public event Action? Pressed;

    public override void _Ready()
    {
        Texture = _normalTexture;
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton button || button.ButtonIndex != MouseButton.Left || !button.Pressed)
        {
            return;
        }
        Pressed?.Invoke();
        GetViewport().SetInputAsHandled();
    }
    
    public bool IsFocused { get; private set; }

    public void LoseFocus()
    {
        Texture = _normalTexture;
        IsFocused = false;
    }

    public void GainFocus()
    {
        Texture = _focusTexture;
        IsFocused = true;
    }

    public void SetTextures(Texture2D normal, Texture2D focus)
    {
        _normalTexture = normal;
        _focusTexture = focus;
    }
    
}