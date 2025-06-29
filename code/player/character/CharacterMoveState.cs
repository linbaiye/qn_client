using System.Collections.Generic;
using Godot;
using NLog;
using QnClient.code.entity;
using QnClient.code.entity.@event;
using QnClient.code.input;
using QnClient.code.util;

namespace QnClient.code.player.character;

public class CharacterMoveState : AbstractCharacterState
{
    private readonly ICharacter _character;
    private readonly double _stateSeconds;
    private double _elapsedSeconds;
    private readonly MoveAction _action;

    private readonly Vector2 _velocity;

    private readonly MoveInput _moveInput;

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private CharacterMoveState(ICharacter character, MoveAction action, MoveInput moveInput)
    {
        _character = character;
        _action = action;
        _elapsedSeconds = 0;
        _stateSeconds = VectorUtil.GetMoveDuration(action);
        _velocity = VectorUtil.VelocityUnit(moveInput.Direction) / (float)_stateSeconds;
        _moveInput = moveInput;
    }

    public override void PhysicProcess(double delta)
    {
        if (_elapsedSeconds == 0)
        {
            _character.Direction = _moveInput.Direction;
            _character.Connection.WriteAndFlush(_moveInput);
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
        _character.EmitEvent(new EntityCoordinateEvent(_character));
        Logger.Debug("Character has moved to {}.", _character.Coordinate);
        if (!_character.MovePressed)
        {
            ChangeToStandState();
            return;
        }
        var moveInput = new MoveInput(_character.GetLocalMousePosition().GetDirection(), _character.Coordinate);
        if (_character.Map.CanMove(_character.Coordinate.Move(moveInput.Direction)))
        {
            _elapsedSeconds = 0;
            var moveAction = ComputeMoveAction(_character, _action);
            _character.ChangeState(new CharacterMoveState(_character, moveAction, moveInput));
            return;
        }
        if (_character.Direction != moveInput.Direction)
        {
            _character.Connection.WriteAndFlush(new TurnInput(moveInput.Direction));
            _character.Direction = moveInput.Direction;
        }
        ChangeToStandState();
    }

    private void ChangeToStandState()
    {
        if (_action == MoveAction.Walk || _action == MoveAction.Run || _action == MoveAction.Fly)
            _character.ChangeState(CharacterStandState.Idle(_character));
        else if (_action == MoveAction.FightWalk)
            _character.ChangeState(CharacterStandState.FightStand(_character));
    }
    
    private static MoveAction ComputeMoveAction(ICharacter character, MoveAction current)
    {
        if (character.FootKungFu != null)
            return character.FootKungFu.CanFly ? MoveAction.Fly : MoveAction.Run;
        return current;
    }

    private static MoveAction ComputeMoveAction(ICharacter character)
    {
        if (character.FootKungFu == null)
            return MoveAction.Walk;
        return character.FootKungFu.CanFly ? MoveAction.Fly : MoveAction.Run;
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