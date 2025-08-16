using System;
using Godot;
using QnClient.code.input;
using QnClient.code.message;
using QnClient.code.util;

namespace QnClient.code.hud.guild;

public partial class ApplyKungFuForm : NinePatchRect
{
    private LineEdit _name;
    private CheckBox _fist;
    private CheckBox _sword;
    private CheckBox _blade;
    private CheckBox _axe;
    private CheckBox _spear;
    private Button _confirmButton;
    private Button _cancelButton;
    private LineEdit _attackSpeed;
    private LineEdit _headDamage;
    private LineEdit _headArmor;
    private LineEdit _swingInnerPower;
    private LineEdit _swingOuterPower;
    private LineEdit _swingPower;
    private LineEdit _swingLife;
    private LineEdit _bodyDamage;
    private LineEdit _armDamage;
    private LineEdit _armArmor;
    private LineEdit _recovery;
    private LineEdit _legDamage;
    private LineEdit _legArmor;
    private LineEdit _avoid;
    private LineEdit _bodyArmor;
    private Label _label;

    public event Action? OnConfirmed;

    public override void _Ready()
    {
        _name = GetNode<LineEdit>("Name");
        _fist = GetNode<CheckBox>("Fist");
        _sword = GetNode<CheckBox>("Sword");
        _blade = GetNode<CheckBox>("Blade");
        _axe = GetNode<CheckBox>("Axe");
        _spear = GetNode<CheckBox>("Spear");
        _fist.Pressed += () => OnChecked(_fist);
        _sword.Pressed += () => OnChecked(_sword);
        _blade.Pressed += () => OnChecked(_blade);
        _spear.Pressed += () => OnChecked(_spear);
        _axe.Pressed += () => OnChecked(_axe);
        _confirmButton = GetNode<Button>("ConfirmButton");
        _cancelButton = GetNode<Button>("CancelButton");
        _attackSpeed = GetNode<LineEdit>("AttackSpeed");
        _headDamage = GetNode<LineEdit>("HeadDamage");
        _headArmor = GetNode<LineEdit>("HeadArmor");
        _swingInnerPower = GetNode<LineEdit>("SwingInnerPower");
        _swingOuterPower = GetNode<LineEdit>("SwingOuterPower");
        _swingPower = GetNode<LineEdit>("SwingPower");
        _swingLife = GetNode<LineEdit>("SwingLife");
        _bodyDamage = GetNode<LineEdit>("BodyDamage");
        _armDamage = GetNode<LineEdit>("ArmDamage");
        _armArmor = GetNode<LineEdit>("ArmArmor");
        _recovery = GetNode<LineEdit>("Recovery");
        _legDamage = GetNode<LineEdit>("LegDamage");
        _legArmor = GetNode<LineEdit>("LegArmor");
        _avoid = GetNode<LineEdit>("Avoid");
        _bodyArmor = GetNode<LineEdit>("BodyArmor");
        _label = GetNode<Label>("Label");
        _confirmButton.Pressed += OnConfirm;
        _cancelButton.Pressed += () => Visible = false;
        _fist.SetPressedNoSignal(true);
        Visible = false;
    }

    private void OnChecked(CheckBox checkBox)
    {
        _fist.SetPressedNoSignal(false);
        _blade.SetPressedNoSignal(false);
        _sword.SetPressedNoSignal(false);
        _axe.SetPressedNoSignal(false);
        _spear.SetPressedNoSignal(false);
        checkBox.SetPressedNoSignal(true);
    }

    private bool CheckDigitInput(LineEdit inputEdit, string field)
    {
        if (string.IsNullOrEmpty(inputEdit.Text) || !inputEdit.DigitOnly())
        {
            _label.Text = field + "只能是数字";
            return false;
        }
        return true;
    }

        
    private bool CheckInputs()
    {
        if (string.IsNullOrEmpty(_name.Text))
        {
            _label.Text = "请输入武功名";
            return false;
        }
        if (!CheckDigitInput(_attackSpeed, "速度"))
        {
            return false;
        }
        if (!CheckDigitInput(_headDamage, "头部攻击"))
        {
            return false;
        }
        if (!CheckDigitInput(_headArmor, "头部防御"))
        {
            return false;
        }
        if (!CheckDigitInput(_swingInnerPower, "内功消耗"))
        {
            return false;
        }
        if (!CheckDigitInput(_swingOuterPower, "外功消耗"))
        {
            return false;
        }
        if (!CheckDigitInput(_swingPower, "武功消耗"))
        {
            return false;
        }
        if (!CheckDigitInput(_swingLife, "活力消耗"))
        {
            return false;
        }
        if (!CheckDigitInput(_bodyDamage, "身体攻击"))
        {
            return false;
        }
        if (!CheckDigitInput(_armDamage, "手臂攻击"))
        {
            return false;
        }
        if (!CheckDigitInput(_armArmor, "手臂防御"))
        {
            return false;
        }
        if (!CheckDigitInput(_recovery, "恢复"))
        {
            return false;
        }
        if (!CheckDigitInput(_legDamage, "腿部攻击"))
        {
            return false;
        }
        if (!CheckDigitInput(_legArmor, "腿部防御"))
        {
            return false;
        }
        if (!CheckDigitInput(_avoid, "闪躲"))
        {
            return false;
        }
        if (!CheckDigitInput(_bodyArmor, "身体防御"))
        {
            return false;
        }
        return true;
    }

    private void Open()
    {
        _attackSpeed.Text = 30.ToString();
        _bodyDamage.Text = 70.ToString();
        _recovery.Text = 50.ToString();
        _avoid.Text = 50.ToString();
        _headDamage.Text = 40.ToString();
        _armDamage.Text = 40.ToString();
        _legDamage.Text = 40.ToString();
        _bodyArmor.Text = 40.ToString();
        _headArmor.Text = 40.ToString();
        _armArmor.Text = 14.ToString();
        _legArmor.Text = 14.ToString();
        _swingInnerPower.Text = 20.ToString();
        _swingOuterPower.Text = 20.ToString();
        _swingPower.Text = 20.ToString();
        _swingLife.Text = 20.ToString();
        Visible = true;
    }

    private int GetAttackKungFuType()
    {
        if (_fist.ButtonPressed)
            return 0;
        if (_sword.ButtonPressed)
            return 1;
        if (_blade.ButtonPressed)
            return 2;
        if (_axe.ButtonPressed)
            return 3;
        if (_spear.ButtonPressed)
            return 4;
        throw new InvalidOperationException();
    }

    public ApplyKungFuInput BuildInput()
    {
        return new ApplyKungFuInput()
        {
            Speed = _attackSpeed.Text.ToInt(),
            Recovery = _recovery.Text.ToInt(),
            Avoid = _avoid.Text.ToInt(),
            BodyDamage = _bodyDamage.Text.ToInt(),
            HeadDamage = _headDamage.Text.ToInt(),
            ArmDamage = _armDamage.Text.ToInt(),
            LegDamage = _legDamage.Text.ToInt(),
            BodyArmor = _bodyArmor.Text.ToInt(),
            HeadArmor = _headArmor.Text.ToInt(),
            ArmArmor = _armArmor.Text.ToInt(),
            LegArmor = _legArmor.Text.ToInt(),
            LifeToSwing = _swingLife.Text.ToInt(),
            PowerToSwing = _swingPower.Text.ToInt(),
            OuterPowerToSwing = _swingOuterPower.Text.ToInt(),
            InnerPowerToSwing = _swingInnerPower.Text.ToInt(),
            Type = GetAttackKungFuType(),
            Name = _name.Text,
        };
    }

    public void ShowMessage(string msg)
    {
        _label.Text = msg;
    }

    private void OnConfirm()
    {
        if (CheckInputs())
        {
            OnConfirmed?.Invoke();
        }
    }
    
}