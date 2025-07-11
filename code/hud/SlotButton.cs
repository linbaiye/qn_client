using System;
using Godot;

namespace QnClient.code.hud;

public partial class SlotButton : TextureButton
{
    public event Action? RightButtonReleased;
    public event Action? LeftButtonDoubleClicked;
    public event Action? LeftButtonReleased;
    public bool MouseHovering { get; private set; }
    private const float DoubleClickThreshold = 0.2f;
    private float _lastClickTime;
    private Timer _timer;
    public override void _Ready()
    {
        MouseEntered += () => MouseHovering = true;
        MouseExited += () => MouseHovering = false;
        _timer = GetNode<Timer>("Timer");
        _timer.OneShot = true;
        _timer.Timeout += OnTimeout;
    }

    private void OnTimeout()
    {
        LeftButtonReleased?.Invoke();
    }
    public override void _GuiInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton button)
        {
            return;
        }
        if (button.ButtonIndex == MouseButton.Right && button.IsReleased())
        {
            RightButtonReleased?.Invoke();
            GetViewport().SetInputAsHandled();
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
                LeftButtonDoubleClicked?.Invoke();
                _timer.Stop();
            }
            else
            {
                _timer.Start(DoubleClickThreshold);
            }
            _lastClickTime = cur;
        }
        GetViewport().SetInputAsHandled();
    }
}
