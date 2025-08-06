using Godot;

namespace QnClient.code.entity;

public partial class Effect : Sprite2D
{
    private Timer _timer;
    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        _timer.OneShot = true;
        _timer.Timeout += () => Visible = false;
        Visible = false;
    }

    public void Show(float seconds)
    {
        if (!_timer.IsStopped())
            return;
        _timer.Start(seconds);
        Visible = true;
    }
}