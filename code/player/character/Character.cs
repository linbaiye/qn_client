using Godot;
using QnClient.code.entity;
using QnClient.code.entity.@event;
using QnClient.code.input;
using QnClient.code.map;
using QnClient.code.message;
using QnClient.code.network;
using QnClient.code.player.character.kungfu;
using QnClient.code.util;

namespace QnClient.code.player.character;

public partial class Character : AbstractPlayer, ICharacter, ICharacterMessageHandler
{
    private ICharacterState? _characterState;
    
    public FootKungFu? FootKungFu { get; private set; }
    public string AttackKungFu { get; private set; }
    public string ProtectionKungFu { get; private set; } = "";
    public string AssistantKungFu { get; private set; } = "";

    private IMap? _map;

    private Connection? _connection;

    public ValueBar LifeBar { get; set; } = ValueBar.Default;
    public ValueBar PowerBar { get; set; } = ValueBar.Default;
    public ValueBar InnerPowerBar { get; set; } = ValueBar.Default;
    public ValueBar OuterPowerBar { get; set; } = ValueBar.Default;
    public ValueBar HeadLifeBar { get; set; } = ValueBar.Default;
    public ValueBar ArmLifeBar { get; set; } = ValueBar.Default;
    public ValueBar LegLifeBar { get; set; } = ValueBar.Default;

    public void ChangeState(ICharacterState state)
    {
        _characterState = state;
    }

    public Connection Connection => _connection;
    
    public IMap Map => _map;

    public bool MovePressed { get; private set; }

    public CreatureDirection Direction { get; set; }

    public override void _PhysicsProcess(double delta)
    {
        _characterState?.PhysicProcess(delta);
    }

    private void HandleMouseEvent(InputEventMouse eventMouse)
    {
        if (eventMouse is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Right)
        {
            if (mouseButton.Pressed)
            {
                _characterState?.Move(new MoveInput(GetLocalMousePosition().GetDirection(), Coordinate));
            }
            MovePressed = mouseButton.Pressed;
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouse eventMouse)
        {
            HandleMouseEvent(eventMouse);
        }
    }

    public void Initialize(JoinRealmMessage message, Connection connection, IMap map)
    {
        _connection = connection;
        _map = map;
        AnimationPlayer.InitializeAnimations(message.Male);
        base.Initialize(message.Id, message.Coordinate, message.Name);
        Direction = CreatureDirection.Down;
        ChangeState(CharacterStandState.Idle(this));
        LifeBar = message.LifeBar;
        HeadLifeBar = message.HeadLifeBar;
        ArmLifeBar = message.ArmLifeBar;
        LegLifeBar = message.LegLifeBar;
        PowerBar = message.PowerBar;
        InnerPowerBar = message.InnerPowerBar;
        OuterPowerBar = message.OuterPowerBar;
        AttackKungFu = message.AttackKungFu;
        foreach (var equipMessage in message.Equipments)
        {
            HandleEntityMessage(equipMessage);
        }
        ResetPhysicsInterpolation();
        EmitEvent(CharacterEvent.Join(this));
        map.Occupy(this);
    }

    public override void HandleEntityMessage(IEntityMessage message)
    {
        if (message is ICharacterMessage characterMessage)
        {
            characterMessage.Accpet(this);
        }
    }
}