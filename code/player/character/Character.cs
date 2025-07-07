using Godot;
using NLog;
using QnClient.code.entity;
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

    private IMap? _map;

    private Connection? _connection;

    
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

    public void ChangeState(ICharacterState state)
    {
        _characterState = state;
    }

    public Connection Connection => _connection;
    
    public IMap Map => _map;

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
            GetViewport().SetInputAsHandled();
        }
    }

    private void HandleKeyInputEvent(InputEventKey eventKey)
    {
        if (eventKey.Pressed)
        {
            SimpleInput? input = null;
            switch (eventKey.Keycode)
            {
                case Key.F2:
                case Key.J:
                    input = SimpleInput.F2;
                    break;
                case Key.F3:
                case Key.K:
                    input = SimpleInput.F3;
                    break;
                case Key.L:
                case Key.F4:
                    input = SimpleInput.F4;
                    break;
            }
            if (input != null)
            {
                _connection?.WriteAndFlush(input);
            }
        }
        GetViewport().SetInputAsHandled();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouse eventMouse)
        {
            HandleMouseEvent(eventMouse);
        }
        else if (@event is InputEventKey eventKey)
        {
            HandleKeyInputEvent(eventKey);
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
        foreach (var equipMessage in message.Equipments)
        {
            HandleEntityMessage(equipMessage);
        }
        ResetPhysicsInterpolation();
        map.Occupy(this);
    }

    public override void HandleEntityMessage(IEntityMessage message)
    {
        if (message is ICharacterMessage characterMessage)
        {
            characterMessage.Accept(this);
        }
    }

    public void ChangeState(PlayerState newState, CreatureDirection direction)
    {
        switch (newState)
        {
            case PlayerState.FightStand:
                _characterState = CharacterStandState.FightStand(this);
                break;
            case PlayerState.Idle:
                _characterState = CharacterStandState.Idle(this);
                break;
            default:
                _characterState = CharacterWaitingState.Instance;
                break;
        }
        PlayStateAnimation(newState, direction);
    }

    public void SetPosition(Vector2I coordinate, PlayerState state, CreatureDirection direction)
    {
        DoSetPosition(coordinate, state, direction);
        if (state == PlayerState.Idle)
            ChangeState(CharacterStandState.Idle(this));
        else if (state == PlayerState.FightStand)
            ChangeState(CharacterStandState.FightStand(this));
        Log.Debug("Position {}.", Position.ToCoordinate());
    }

    public void Attack(AttackAction action, CreatureDirection direction, string effect)
    {
        PlayAttackAnimation(action, direction, effect);
        ChangeState(CharacterWaitingState.Instance);
    }

    public void SyncActiveKungFuList(SyncActiveKungFuListMessage message) 
    {
        FootKungFu = message.FootKungFu;
    }
}