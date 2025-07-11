using System;
using Godot;
using QnClient.code.message;
using QnClient.code.player;

namespace QnClient.code.hud.bottom;

public partial class Bottom : NinePatchRect
{
    private TextureProgressBar _lifeBar;
    private TextureProgressBar _powerBar;
    private TextureProgressBar _outerPowerBar;
    private TextureProgressBar _innerPowerBar;
    private TextureProgressBar _headLifeBar;
    private TextureProgressBar _armLifeBar;
    private TextureProgressBar _legLifeBar;
    private Label _coordinate;
    private Label _mapName;
    private ActiveKungFuList _activeKungFuList;

    private BlinkingLabel _blinkingLabel;

    public event Action? InventoryButtonPressed;
    public event Action? KungFuBookButtonPressed;
    public event Action? AssistanceButtonPressed;
    public event Action? SystemButtonPressed;
    
    private TextArea _textArea;

    private EquipView _equipView;
    
    public event Action<EquipmentType>? UnequipPressed;

    public override void _Ready()
    {
        _lifeBar = GetNode<TextureProgressBar>("LifeBar");
        _powerBar = GetNode<TextureProgressBar>("PowerBar");
        _innerPowerBar = GetNode<TextureProgressBar>("InnerPower");
        _outerPowerBar = GetNode<TextureProgressBar>("OuterPower");
        _headLifeBar = GetNode<TextureProgressBar>("HeadLifeBar");
        _armLifeBar = GetNode<TextureProgressBar>("ArmLifeBar");
        _legLifeBar = GetNode<TextureProgressBar>("LegLifeBar");
        _coordinate = GetNode<Label>("Coordinate");
        _mapName = GetNode<Label>("MapName");
        _textArea = GetNode<TextArea>("TextArea");
        _activeKungFuList = GetNode<ActiveKungFuList>("ActiveKungFuList");
        _blinkingLabel = GetNode<BlinkingLabel>("BlinkingLable");
        GetNode<Button>("Inventory").Pressed += () => InventoryButtonPressed?.Invoke();
        GetNode<Button>("KungFu").Pressed += () => KungFuBookButtonPressed?.Invoke();
        GetNode<Button>("Assistance").Pressed += () => AssistanceButtonPressed?.Invoke();
        GetNode<Button>("System").Pressed += () => SystemButtonPressed?.Invoke();
        _equipView = GetNode<EquipView>("EquipView");
        _equipView.UnequipPressed += type => UnequipPressed?.Invoke(type);
    }

    private void FillBar(TextureProgressBar bar, int value, string tooltip)
    {
        bar.Value = value;
        bar.TooltipText = tooltip;
    }

    public void SyncActiveKungFuList(SyncActiveKungFuListMessage message)
    {
        _activeKungFuList.SyncActiveKungFu(message);
    }

    public void DisplayText(string text)
    {
        _textArea.Display(text);
    }

    public void BlinkKungFu(string name)
    {
        _activeKungFuList.BlinkKungFu(name);
    }

    public void BlinkText(string text)
    {
        _blinkingLabel.BlinkThenHide(text);
    }

    public void UpdateCoordinate(Vector2I coordinate)
    {
        _coordinate.Text = coordinate.X + ":" + coordinate.Y;
    }

    public void OnCharacterJoined(JoinRealmMessage message)
    {
        FillBar(_lifeBar, message.LifeBar.Percent, message.LifeBar.Text);
        FillBar(_powerBar , message.PowerBar.Percent, message.PowerBar.Text);
        FillBar(_innerPowerBar, message.InnerPowerBar.Percent, message.InnerPowerBar.Text);
        FillBar(_outerPowerBar, message.OuterPowerBar.Percent, message.OuterPowerBar.Text);
        FillBar(_headLifeBar, message.HeadLifeBar.Percent, message.HeadLifeBar.Percent.ToString());
        FillBar(_armLifeBar, message.ArmLifeBar.Percent, message.ArmLifeBar.Percent.ToString());
        FillBar(_legLifeBar, message.LegLifeBar.Percent, message.LegLifeBar.Percent.ToString());
        UpdateCoordinate(message.Coordinate);
        _activeKungFuList.SetAttackKungFu(message.AttackKungFu);
        _mapName.Text = message.ResourceName;
        _equipView.OnCharacterJoined(message);
    }

    public void UpdateAttribute(AttributeMessage message)
    {
        FillBar(_lifeBar, message.Health.Percent, message.Health.Text);
        FillBar(_powerBar , message.Power.Percent, message.Power.Text);
        FillBar(_innerPowerBar, message.InnerPower.Percent, message.InnerPower.Text);
        FillBar(_outerPowerBar, message.OuterPower.Percent, message.OuterPower.Text);
        FillBar(_headLifeBar, message.HeadPercent, message.HeadPercent.ToString());
        FillBar(_armLifeBar, message.ArmPercent, message.ArmPercent.ToString());
        FillBar(_legLifeBar, message.LegPercent, message.LegPercent.ToString());
    }
    
    public void UpdateLifeBars(PlayerDamagedMessage message)
    {
        FillBar(_lifeBar, message.LifeBar.Percent, message.LifeBar.Text);
        FillBar(_headLifeBar, message.Head, message.Head.ToString());
        FillBar(_armLifeBar, message.Arm, message.Arm.ToString());
        FillBar(_legLifeBar, message.Leg, message.Leg.ToString());
    }

    public void Unequip(EquipmentType type)
    {
        _equipView.Unequip(type);
    }

    public void Equip(EquipmentType type, string prefix, string name, int color = 0, string pairedPrefix = null)
    {
        _equipView.Equip(type, prefix, name, color, pairedPrefix);
    }
}