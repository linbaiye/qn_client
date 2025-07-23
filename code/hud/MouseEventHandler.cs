using System;
using Godot;
using NLog;

namespace QnClient.code.hud;

public partial class MouseEventHandler : Timer
{
    private const float DoubleClickThreshold = 0.2f;
    
    private float _lastClickTime;
    
    public event Action? DoubleClicked;
    
    public event Action? Clicked;
    
    public event Action? RightClicked;
    
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
    public override void _Ready()
    {
        OneShot = true;
        Timeout += () =>
        {
            Clicked?.Invoke();
        };
    }
    
    public void HandleMouseButton(InputEventMouseButton button)
    {
        if (button.ButtonIndex == MouseButton.Right && button.IsReleased())
        {
            RightClicked?.Invoke();
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
                Stop();
                DoubleClicked?.Invoke();
            }
            else
            {
                Start(DoubleClickThreshold);
            }
            _lastClickTime = cur;
        }
    }
}