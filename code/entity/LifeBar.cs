using Godot;

namespace QnClient.code.entity;

public partial class LifeBar: TextureProgressBar
{
    private Timer _timer;
    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        _timer.OneShot = true;
        _timer.Timeout += () => Visible = false;
        Visible = false;
    }
    public void Show(int value)
    {
        if (!Visible)
            Position = new Vector2(18 - Size.X / 2, -42);
        Value = value;
        _timer.Stop();
        Visible = true;
        _timer.Start(3);
    }
}