using System;
using Godot;
using QnClient.code.entity.@event;
using QnClient.code.player.character;

namespace QnClient.code.ui.bottom;

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

    public event Action? InventoryButtonPressed;
    public event Action? KungFuBookButtonPressed;
    public event Action? AssistanceButtonPressed;
    public event Action? SystemButtonPressed;
    private TextArea _textArea;

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
        GetNode<Button>("Inventory").Pressed += () => InventoryButtonPressed?.Invoke();
        GetNode<Button>("KungFu").Pressed += () => KungFuBookButtonPressed?.Invoke();
        GetNode<Button>("Assistance").Pressed += () => AssistanceButtonPressed?.Invoke();
        GetNode<Button>("System").Pressed += () => SystemButtonPressed?.Invoke();
    }

    private void FillBar(TextureProgressBar bar, int value, string tooltip)
    {
        bar.Value = value;
        bar.TooltipText = tooltip;
    }

    private void FillBars(ICharacter character)
    {
        FillBar(_lifeBar, character.LifeBar.Percent, character.LifeBar.Text);
        FillBar(_powerBar , character.PowerBar.Percent, character.PowerBar.Text);
        FillBar(_innerPowerBar, character.InnerPowerBar.Percent, character.InnerPowerBar.Text);
        FillBar(_outerPowerBar, character.OuterPowerBar.Percent, character.OuterPowerBar.Text);
        FillBar(_headLifeBar, character.HeadLifeBar.Percent, character.HeadLifeBar.Percent.ToString());
        FillBar(_armLifeBar, character.ArmLifeBar.Percent, character.ArmLifeBar.Percent.ToString());
        FillBar(_legLifeBar, character.LegLifeBar.Percent, character.LegLifeBar.Percent.ToString());
    }

    public void Handle(CharacterEvent characterEvent)
    {
        if (characterEvent.Type == CharacterEvent.EventType.Joined)
        {
            FillBars(characterEvent.Character);
            UpdateCoordinate(characterEvent.Character.Coordinate);
            _mapName.Text = characterEvent.Character.Map.Name;
            _activeKungFuList.Update(characterEvent.Character);
        }
    }

    public void DisplayText(string text)
    {
        _textArea.Display(text);
    }

    public void UpdateCoordinate(Vector2I coordinate)
    {
        _coordinate.Text = coordinate.X + ":" + coordinate.Y;
    }
}