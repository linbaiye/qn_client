
using Godot;
using QnClient.code.creature;
using QnClient.code.input;
using QnClient.code.map;
using QnClient.code.message;
using QnClient.code.player.kungfu;
using QnClient.code.util;

namespace QnClient.code.player;

public partial class Character : AbstractPlayer, ICharacter, IMessageHandler
{
    private ICharacterState _characterState;
    
    public FootKungFu? FootKungFu { get; private set; }
    
    private IMap? _map;
    
    public void ChangeState(ICharacterState state)
    {
        _characterState = state;
    }

    public void SetMap(IMap map)
    {
        _map ??= map;
    }
    
    public IMap Map => _map;

    public bool MovePressed { get; private set; }


    public override void _Ready()
    {
        base._Ready();
        Direction = CreatureDirection.Down;
        _characterState = CharacterStandState.Idle(this);
        FootKungFu = new FootKungFu(true);
    }

    public override void _Process(double delta)
    {
        _characterState.Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        _characterState.PhysicProcess(delta);
    }

    private void HandleMouseEvent(InputEventMouse eventMouse)
    {
        if (eventMouse is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Right)
        {
            if (mouseButton.Pressed)
            {
                _characterState.Move(new MoveInput(GetLocalMousePosition().GetDirection()));
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

    public void Handle(JoinRealmMessage message)
    {
        Position = message.Coordinate.ToPosition();
        Id = message.Id;
        EntityName = message.Name;
        ResetPhysicsInterpolation();
    }
}