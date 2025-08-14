using System;
using System.Linq;
using Godot;

namespace QnClient.code.hud.inventory;

public partial class ItemModifyInput : AbstractInputWindow
{
    private Label _name;
    
    public Action<ItemModifyInput>? Confirmed { get; set; }

    public override void _Ready()
    {
        base._Ready();
        _name = GetNode<Label>("Name");
        SetInUse(false);
    }

    protected override void OnConfirmPressed()
    {
        if (!IsNumberOk)
        {
            ShowError("请输入正确数量。");
            return;
        }
        Confirmed?.Invoke(this);
    }

    protected override void OnCancelPressed()
    {
        SetInUse(false);
    }

    public void SetInUse(bool use)
    {
        Visible = use;
        ClearExtra();
        ClearError();
        Input.Editable = true;
    }

    private void OnConfirmed()
    {
        Confirmed?.Invoke(this);
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (!Visible)
            return;
        if (@event is InputEventKey key && (key.Keycode == Key.Enter || key.Keycode == Key.KpEnter))
        {
            OnConfirmed();
            GetViewport().SetInputAsHandled();
        }
    }

    private bool IsNumberOk
    {
        get
        {
            if (string.IsNullOrEmpty(Input.Text)|| !Input.Text.Trim().All(char.IsDigit))
            {
                return false;
            }
            return int.Parse(Input.Text.Trim()) > 0;
        }
    }

    public int Number
    {
        get
        {
            if (!IsNumberOk)
                throw new Exception("Bad input.");
            return int.Parse(Input.Text.Trim());
        }
    }

    public void ToggleEditable(bool flag)
    {
        Input.Editable = flag;
    }

    public void SetNameNumber(string name, long number)
    {
        _name.Text = name;
        Input.Text = number > 0 ? number.ToString() : null;
    }
    
    public void SetNameNumberFocus(string name, long number)
    {
        SetNameNumber(name, number);
        Input.GrabFocus();
    }

    public static ItemModifyInput Create()
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://scene/ui/item_modify_input.tscn");
        return scene.Instantiate<ItemModifyInput>();
    }
}