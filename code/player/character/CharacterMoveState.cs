using System;
using Godot;
using NLog;
using QnClient.code.entity;
using QnClient.code.entity.@event;
using QnClient.code.input;
using QnClient.code.message;
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
    
    private readonly Vector2 _destination;
        
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    
    private CharacterMoveState(ICharacter character, MoveAction action, MoveInput moveInput, double elapsedSeconds = 0)
    {
        _character = character;
        _action = action;
        _elapsedSeconds = elapsedSeconds;
        _stateSeconds = VectorUtil.GetMoveDuration(action);
        _velocity = VectorUtil.VelocityUnit(moveInput.Direction) / (float)_stateSeconds;
        _moveInput = moveInput;
        _character.Position += _velocity * (float)elapsedSeconds;
        _destination = moveInput.Destination;
    }


    // 移动中承受攻击会导致服务器和客户端移动位置错位，需要自行判断是否到达目的地。
    private bool ReachDestination(Vector2 nextPosition)
    {
        switch (_moveInput.Direction)
        {
            case CreatureDirection.Left:
                return nextPosition.X <= _destination.X;
            case CreatureDirection.Right:
                return nextPosition.X >= _destination.X;
            case CreatureDirection.Up:
                return nextPosition.Y <= _destination.Y;
            case CreatureDirection.Down:
                return nextPosition.Y >= _destination.Y;
            case CreatureDirection.UpLeft:
                return nextPosition.Y <= _destination.Y && nextPosition.X <= _destination.X;
            case CreatureDirection.UpRight:
                return nextPosition.Y <= _destination.Y && nextPosition.X >= _destination.X;
            case CreatureDirection.DownLeft:
                return nextPosition.Y >= _destination.Y && nextPosition.X <= _destination.X;
            case CreatureDirection.DownRight:
                return nextPosition.Y >= _destination.Y && nextPosition.X >= _destination.X;
        }
        return false;
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
        var nextPosition = _character.Position + (float)delta * _velocity;
        if (!ReachDestination(nextPosition) && _elapsedSeconds + delta < _stateSeconds)
        {
            _elapsedSeconds += delta;
            _character.Position = nextPosition;
            return;
        }
        _character.Position = _moveInput.Destination;
        _character.EmitEvent(new EntityChangeCoordinateEvent(_character));
        if (_character.NextMoveDirection == null)
        {
            ChangeToStandState();
            return;
        }
        var moveInput = new MoveInput(_character.NextMoveDirection.Value, _character.Coordinate);
        if (_character.Map.CanMove(_character.Coordinate.Move(moveInput.Direction)))
        {
            var moveAction = ComputeMoveAction(_character, _action);
            _character.ChangeState(new CharacterMoveState(_character, moveAction, moveInput));
            _character.PlayMoveAnimation(moveAction, moveInput.Direction);
            return;
        }
        if (_character.Direction != moveInput.Direction)
        {
            _character.Connection.WriteAndFlush(new TurnInput(moveInput.Direction));
            _character.Direction = moveInput.Direction;
        }
        ChangeToStandState();
    }

    public override void Teleported()
    {
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
        if (character.FootKungFu == null)
        {
            return current == MoveAction.FightWalk ? current : MoveAction.Walk;
        }
        return character.FootKungFu.CanFly ? MoveAction.Fly : MoveAction.Run;
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

    public static CharacterMoveState Restore(ICharacter character, MoveMessage message)
    {
        var input = new MoveInput(message.Direction, message.Start);
        return new CharacterMoveState(character, message.Action.Value, input, (float)message.StartMillis / 1000);
    }
    
    public static CharacterMoveState FightWalk(ICharacter character, MoveInput moveInput)
    {
        return new CharacterMoveState(character, MoveAction.FightWalk, moveInput);
    }
}