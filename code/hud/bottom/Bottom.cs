using System;
using Godot;
using QnClient.code.hud.hotkey;
using QnClient.code.hud.inventory;
using QnClient.code.hud.kungfu;
using QnClient.code.input;
using QnClient.code.message;
using QnClient.code.network;
using QnClient.code.player;

namespace QnClient.code.hud.bottom;

public partial class Bottom : NinePatchRect, ICharacterJoinedAware, IAttributeProvider, IConnectionAware
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

    private Connection _connection;

    public event Action? InventoryButtonPressed;
    public event Action? KungFuBookButtonPressed;
    public event Action? AssistanceButtonPressed;
    public event Action? SystemButtonPressed;

    public event Action? PlayerAvatarPressed;
    
    private TextArea _textArea;

    private EquipView _equipView;
    private TextureProgressBar _expUp;
    private TextureProgressBar _expDown;

    private HotKeyManager _hotKeyManager;

    private LineEdit _chat;
    
    
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
        _textArea.PrivateChatPressed += OnPrivateChatPressed;
        _activeKungFuList = GetNode<ActiveKungFuList>("ActiveKungFuList");
        _blinkingLabel = GetNode<BlinkingLabel>("BlinkingLable");
        GetNode<Button>("Inventory").Pressed += () => InventoryButtonPressed?.Invoke();
        GetNode<Button>("KungFu").Pressed += () => KungFuBookButtonPressed?.Invoke();
        GetNode<Button>("Assistance").Pressed += () => AssistanceButtonPressed?.Invoke();
        GetNode<Button>("System").Pressed += () => SystemButtonPressed?.Invoke();
        _equipView = GetNode<EquipView>("EquipView");
        _equipView.AvatarPressed += () => PlayerAvatarPressed?.Invoke();
        _expUp = GetNode<TextureProgressBar>("ExpUp");
        _expDown = GetNode<TextureProgressBar>("ExpDown");
        _chat = GetNode<LineEdit>("Chat");
        _chat.TextSubmitted += OnChatSubmitted;
    }

    private void OnPrivateChatPressed(string characterName)
    {
        _chat.Text = "@纸条 " + characterName + " ";
        _chat.CaretColumn  = _chat.Text.Length;
        _chat.ReleaseFocus();
        _chat.GrabFocus();
    }

    public void SetBookAndInventory(KungFuBook kungFuBook, Inventory inventory)
    {
        _hotKeyManager = new HotKeyManager(GetNode<HBoxContainer>("LeftHotKeys"),
            GetNode<HBoxContainer>("RightHotKeys"), inventory, kungFuBook);
    }
    
    private void OnChatSubmitted(string text) 
    {
        if (string.IsNullOrWhiteSpace(text))
            return;
        _connection?.WriteAndFlush(new ChatInput(text));
        _chat.Text = null;
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
        _textArea.Display(text, null, null);
    }

    public void DisplayText(TextMessage message)
    {
        _textArea.Display(message);
    }

    public void SetTextHistoryWindow(TextHistoryWindow window)
    {
        _textArea.SetTextHistoryWindow(window);
    }

    public void DisplayText(string text, string color, string bgColor)
    {
        _textArea.Display(text, color, bgColor);
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

    public void UpdateExpBar(int level)
    {
        if (level == 0)
            return;
        _expUp.Value = level % 100;
        _expDown.Value = level / 100;
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
        _mapName.Text = message.MapTile;
        _equipView.Display(message.Male, message.Equipments);
        UpdateExpBar(message.AttackKungFuLevel);
        _hotKeyManager.OnCharacterJoined();
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

    public void OnCharacterTeleported(string mapTitle, Vector2I coordinate)
    {
        _mapName.Text = mapTitle;
        UpdateCoordinate(coordinate);
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

    public void Equip(PlayerEquipMessage message)
    {
        _equipView.Equip(message);
    }
    
    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (_hotKeyManager.HandleKeyEvent(@event))
        {
            GetViewport().SetInputAsHandled();
        }
        else if (@event is InputEventKey key && key.Pressed && key.Keycode == Key.Enter)
        {
            _chat.GrabFocus();
            GetViewport().SetInputAsHandled();
        }
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            _hotKeyManager.Save();
        }
    }

    public int GetPercent(AttributeType type)
    {
        return type switch
        {
            AttributeType.Life => (int)_lifeBar.Value,
            AttributeType.Power => (int)_powerBar.Value,
            AttributeType.InnerPower => (int)_innerPowerBar.Value,
            AttributeType.OutPower => (int)_outerPowerBar.Value,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }


    private void UnequipPressed(EquipmentType t)
    {
        _connection?.WriteAndFlush(new UnequipInput(t));
    }

    public void SetConnection(Connection connection)
    {
        _connection = connection;
    }
}