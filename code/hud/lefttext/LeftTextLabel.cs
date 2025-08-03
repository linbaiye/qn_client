using System;
using System.Text.RegularExpressions;
using Godot;

namespace QnClient.code.hud.lefttext;

public partial class LeftTextLabel : Label
{
    private Timer _timer;

    public event Action<int>? Timeout;
    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        _timer.OneShot = true;
        _timer.Timeout += OnTimeOut;
    }

    private void OnTimeOut()
    {
        var match = Regex.Match(GetName(), "(\\d+)");
        var number = !match.Success ? -1 : int.Parse(match.Groups[1].Value);
        if (number != -1)
            Timeout?.Invoke(number);
    }
    
    public void SetContent(string text)
    {
        Visible = true;
        Text = text;
        _timer.Start(3f);
    }

    public void Clear()
    {
        _timer.Stop();
        Text = null;
        Visible = false;
    }
    
    public bool Empty => string.IsNullOrEmpty(Text);

    public void Copy(LeftTextLabel another)
    {
        Visible = true;
        Text = another.Text;
        if (another._timer.TimeLeft > 0)
        {
            _timer.Stop();
            _timer.Start(another._timer.TimeLeft);
        }
    }
}