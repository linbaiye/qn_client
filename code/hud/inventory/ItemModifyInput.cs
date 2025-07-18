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

    private int _maxNumber;
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

    private bool _using;
    public bool Using => _using;
    
    public void SetInUse(bool use)
    {
        _using = use;
        Visible = _using;
        _extra.Clear();
        _error.Text = null;
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
            if (!_input.Text.All(char.IsDigit))
            {
                return false;
            }
            var number = int.Parse(_input.Text);
            return number > 0 && number <= _maxNumber;
        }
    }

    public int Number
    {
        get
        {
            if (!IsNumberOk)
                throw new Exception("Bad input.");
            return int.Parse(_input.Text);
        }
    }

    public void SetNameAndNumber(string name, int number)
    {
        _name.Text = name;
        _input.Text = number.ToString();
        _maxNumber = number;
    }
        
    public void SetExtra(string key, object value)
    {
        _extra.Remove(key);
        _extra.Add(key, value);
    }

}