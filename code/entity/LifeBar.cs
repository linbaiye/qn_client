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
    public void Show(int value, Vector2 xcenterY)
    {
        Position = new Vector2(xcenterY.X - Size.X / 2, xcenterY.Y - 6);
        _timer.Stop();
        Visible = true;
        _timer.Start(3);
    }
}