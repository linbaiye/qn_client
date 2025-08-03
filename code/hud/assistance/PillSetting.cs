using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using Godot;
using QnClient.code.util;

namespace QnClient.code.hud.assistance;

public partial class PillSetting : Control
{
    private CheckBox _check;
    private LineEdit _input;
    private OptionButton _itemList;
    public event Action<int>? DropdownPressed;
    
    public override void _Ready()
    {
        _check = GetNode<CheckBox>("CheckBox");
        _input = GetNode<LineEdit>("Input");
        _itemList = GetNode<OptionButton>("ItemList");
        _itemList.Pressed += OnDropdownPressed;
    }

    private void OnDropdownPressed()
    {
        var match = Regex.Match(GetName(), "(\\d+)");
        var number = !match.Success ? -1 : int.Parse(match.Groups[1].Value);
        if (number != -1)
            DropdownPressed?.Invoke(number);
    }

    public string PillName
    {
        get
        {
            var number = _itemList.GetSelected();
            return number == -1 ? "" : _itemList.GetItemText(number);
        }
    }

    public int Percent => IsNumberOk ? int.Parse(_input.Text.Trim()) : -1;
    
    private bool IsNumberOk
    {
        get
        {
            if (!_input.DigitOnly())
            {
                return false;
            }
            int v = int.Parse(_input.Text.Trim());
            return v > 0 && v < 100;
        }
    }

    public bool Active
    {
        get
        {
            var number = _itemList.GetSelected();
            if (number == -1)
                return false;
            return IsNumberOk && _check.ButtonPressed;
        }
    }
    
    private class JsonObject
    {
        public bool Checked { get; set; }
        public string Selected { get; set; }
        public int Percent { get; set; }
    }

    public string Serialize
    {
        get
        {
            JsonObject jsonObject = new JsonObject()
            {
                Checked = _check.ButtonPressed,
                Selected = PillName,
                Percent = Percent,
            };
            return JsonSerializer.Serialize(jsonObject);
        }
    }


    public void Deserialize(string conf)
    {
        var jsonObject = JsonSerializer.Deserialize<JsonObject>(conf);
        if (jsonObject.Checked)
            _check.ButtonPressed = true;
        if (!string.IsNullOrEmpty(jsonObject.Selected))
        {
            _itemList.AddItem(jsonObject.Selected);
            _itemList.Select(0);
        }
        if (jsonObject.Percent != -1)
            _input.Text = jsonObject.Percent.ToString();
    }
    
    public void FillItems(List<string> items)
    {
        _itemList.Clear();
        items.ForEach(item => _itemList.AddItem(item));
    }
}