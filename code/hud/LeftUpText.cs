using Godot;

namespace QnClient.code.hud;

public partial class LeftUpText : NinePatchRect
{
    private Label _label;
    private Timer _timer;
    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
        _timer = GetNode<Timer>("Timer");
        _timer.OneShot = true;
        _timer.Timeout += () => Visible = false;
        Visible = false;
    }

    public void Display(string content)
    {
        _timer.Stop();
        _label.Text = content;
        _timer.Start(5);
        Visible = true;
    }
}