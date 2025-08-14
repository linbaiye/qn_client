using System;
using Godot;

namespace QnClient.code.hud;

public partial class SimpleInputWindow : AbstractInputWindow
{
    private Label _title;
    private Label _tip;

    public event Action? Confirmed;
    public event Action? Cancelled;
        
    public override void _Ready()
    {
        base._Ready();
        _title = GetNode<Label>("Title");
        _tip = GetNode<Label>("Tip");
        Visible = false;
    }

    protected override void OnConfirmPressed()
    {
        Confirmed?.Invoke();
    }

    public void SetTitleTip(string t, string tip)
    {
        _title.Text = t;
        _tip.Text = tip;
        Visible = true;
    }

    public void Clear()
    {
        _title.Text = null;
        _tip.Text = null;
        Visible = false;
    }

    public string GuildName => Input.Text;

    protected override void OnCancelPressed()
    {
        Cancelled?.Invoke();
    }
}