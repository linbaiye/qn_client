using System.Collections;
using System.Collections.Generic;
using Godot;
using QnClient.code.util;

namespace QnClient.code.player;

public class CharacterMoveState : ICharacterState
{
    private readonly ICharacter _character;
    private readonly double _stateSeconds;
    private double _elapsedSeconds;
    private readonly MoveAction _action;

    private readonly Vector2 _velocity;

    private readonly MoveInput _moveInput;

    private static readonly Dictionary<MoveAction, double> StateSeconds = new()
    {
        { MoveAction.Walk, 0.84 },
        { MoveAction.FightWalk, 0.84 },
        { MoveAction.Run, 0.42 },
        { MoveAction.Fly, 0.36 },
    };

    private CharacterMoveState(ICharacter character, MoveAction action, MoveInput moveInput)
    {
        _character = character;
        _action = action;
        StateSeconds.TryGetValue(action, out _stateSeconds);
        _elapsedSeconds = 0;
        _velocity = VectorUtil.VelocityUnit(moveInput.Direction) / (float)_stateSeconds;
        _moveInput = moveInput;
    }

    public void Move(MoveInput input)
    {
    }

    public void PhysicProcess(double delta)
    {
        if (_elapsedSeconds == 0)
        {
            _character.Direction = _moveInput.Direction;
            _character.AnimationPlayer.Stop();
            if (_action == MoveAction.Walk)
                _character.AnimationPlayer.PlayWalk(_character.Direction);
            else if (_action == MoveAction.Run)
                _character.AnimationPlayer.PlayRun(_character.Direction);
            else if (_action == MoveAction.FightWalk)
                _character.AnimationPlayer.PlayFightWalk(_character.Direction);
            else if (_action == MoveAction.Fly)
                _character.AnimationPlayer.PlayFly(_character.Direction);
        }
        _elapsedSeconds += delta;
        _character.Position += (float)delta * _velocity;
        if (_elapsedSeconds < _stateSeconds)
            return;
        _character.Position = _character.Position.Snapped(VectorUtil.TileSize);
        if (_character.MovePressed)
        {
            _elapsedSeconds = 0;
            var moveInput = new MoveInput(_character.GetLocalMousePosition().GetDirection());
            var moveAction = ComputeMoveAction(_character);
            _character.ChangeState(new CharacterMoveState(_character, moveAction, moveInput));
            return;
        }
        if (_action == MoveAction.Walk || _action == MoveAction.Run || _action == MoveAction.Fly)
            _character.ChangeState(CharacterStandState.Idle(_character));
        else if (_action == MoveAction.FightWalk)
            _character.ChangeState(CharacterStandState.FightStand(_character));
    }

    private static MoveAction ComputeMoveAction(ICharacter character)
    {
        if (character.FootKungFu == null)
            return MoveAction.Walk;
        return character.FootKungFu.CanFly ? MoveAction.Fly : MoveAction.Run;
    }

    public void Process(double delta)
    {
    }
    
    public static CharacterMoveState Move(ICharacter character, MoveInput moveInput)
    {
        return new CharacterMoveState(character, ComputeMoveAction(character), moveInput);
    }
    
    public static CharacterMoveState FightWalk(ICharacter character, MoveInput moveInput)
    {
        return new CharacterMoveState(character, MoveAction.FightWalk, moveInput);
    }
}