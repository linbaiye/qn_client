using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace QnClient.code.hud.inventory;

public partial class ItemModifyInput : NinePatchRect
{
    private Label _name;
    
    private LineEdit _input;
    private Button _confirm;
    private Button _cancel;
    private Label _error;

    private readonly Dictionary<string, object> _extra = new();
    public Action<ItemModifyInput>? Confirmed { get; set; }

    public override void _Ready()
    {
        _name = GetNode<Label>("Name");
        _input = GetNode<LineEdit>("NumberInput");
        _confirm = GetNode<Button>("Confirm");
        _cancel = GetNode<Button>("Cancel");
        _confirm.Pressed += OnConfirmed;
        _cancel.Pressed += () => SetInUse(false);
        _error = GetNode<Label>("Error");
        SetInUse(false);
    }

    public void SetInUse(bool use)
    {
        Visible = use;
        _extra.Clear();
        _error.Text = null;
        _input.Editable = true;
    }

    private void OnConfirmed()
    {
        if (!IsNumberOk)
        {
            _error.Text = "请输入正确数量。";
            return;
        }
        Confirmed?.Invoke(this);
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (!Visible)
            return;
        if (@event is InputEventKey key && key.Keycode == Key.Enter)
        {
            OnConfirmed();
            GetViewport().SetInputAsHandled();
        }
    }


    public T? GetExtra<T>(string key)
    {
        if (_extra.TryGetValue(key, out var obj))
        {
            if (obj is T t)
                return t;
        }
        return default;
    }

    private bool IsNumberOk
    {
        get
        {
            if (string.IsNullOrEmpty(_input.Text)|| !_input.Text.Trim().All(char.IsDigit))
            {
                return false;
            }
            return int.Parse(_input.Text.Trim()) > 0;
        }
    }

    public int Number
    {
        get
        {
            if (!IsNumberOk)
                throw new Exception("Bad input.");
            return int.Parse(_input.Text.Trim());
        }
    }

    public void ToggleEditable(bool flag)
    {
        _input.Editable = flag;
    }

    public void SetNameNumber(string name, long number)
    {
        _name.Text = name;
        _input.Text = number > 0 ? number.ToString() : null;
    }
    
    public void SetNameNumberFocus(string name, long number)
    {
        SetNameNumber(name, number);
        _input.GrabFocus();
    }
    
        
    public void SetExtra(string key, object value)
    {
        _extra.Remove(key);
        _extra.Add(key, value);
    }
    
    public static ItemModifyInput Create()
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://scene/ui/item_modify_input.tscn");
        return scene.Instantiate<ItemModifyInput>();
    }
}