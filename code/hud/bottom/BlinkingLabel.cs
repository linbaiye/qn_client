using System;
using Godot;

namespace QnClient.code.hud.bottom;

public partial class BlinkingLabel : RichTextLabel
{
    private Timer _timer;

    private string _text;


    private Action _timeoutAction;

    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        _timer.OneShot = true;
        _timer.Timeout += () => _timeoutAction?.Invoke();
    }

    private string CenterText => "[center]" + _text + "[/center]";

    public void SetKungFuName(string text)
    {
        _timer.Stop();
        _text = text;
        Text = CenterText;
    }

    private void StopAndHide()
    {
        ProcessMode = ProcessModeEnum.Disabled;
        Text = "";
    }

    private void StopBlink()
    {
        ProcessMode = ProcessModeEnum.Disabled;
        Text = CenterText;
    }


    private void Blink(string text, Action action)
    {
        ProcessMode = ProcessModeEnum.Inherit;
        _text = text;
        _timer.Stop();
        _timeoutAction = action;
        Text = "[pulse freq=8 color=#6193df ease=-2.0]" + CenterText + "[/pulse]";
        _timer.Start(1.5);
    }

    public void BlinkThenHide(string text)
    {
        Blink(text, StopAndHide);
    }

    public bool BlinkIfMatches(string text)
    {
        if (text.Equals(_text))
        {
            Blink(text, StopBlink);
            return true;
        }
        return false;
    }
}