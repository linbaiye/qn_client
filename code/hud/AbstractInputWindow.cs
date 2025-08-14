using System.Collections.Generic;
using Godot;

namespace QnClient.code.hud;

public abstract partial class AbstractInputWindow : NinePatchRect
{
    protected LineEdit Input;
    private Button _confirm;
    private Button _cancel;
    private Label _error;
    private readonly Dictionary<string, object> _extra = new();
    public override void _Ready()
    {
        Input = GetNode<LineEdit>("Input");
        _confirm = GetNode<Button>("Confirm");
        _cancel = GetNode<Button>("Cancel");
        _error = GetNode<Label>("Error");
        _cancel.Pressed += OnCancelPressed;
        _confirm.Pressed += OnConfirmPressed;
    }

    protected void ClearError()
    {
        _error.Text = null;
    }
    public void ShowError(string error)
    {
        _error.Text = error;
    }
    
    protected abstract void OnConfirmPressed();
    
    protected abstract void OnCancelPressed();

    
    public T? GetExtra<T>(string key)
    {
        if (_extra.TryGetValue(key, out var obj))
        {
            if (obj is T t)
                return t;
        }
        return default;
    }

    protected void ClearExtra()
    {
        _extra.Clear();
    }
    
    public void SetExtra(string key, object value)
    {
        _extra.Remove(key);
        _extra.Add(key, value);
    }

}