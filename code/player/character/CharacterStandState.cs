using QnClient.code.input;
using QnClient.code.util;

namespace QnClient.code.player.character;

public class CharacterStandState : AbstractCharacterState
{
    private readonly ICharacter _character;

    private readonly PlayerState _state;

    private CharacterStandState(ICharacter character, PlayerState state)
    {
        _character = character;
        if (state == PlayerState.Idle)
            character.AnimationPlayer.PlayIdle(character.Direction);
        else if (state == PlayerState.FightStand)
            character.AnimationPlayer.PlayFightStand(character.Direction);
        _state = state;
    }
    
    public override void Move(MoveInput input)
    {
        if (!_character.Map.CanMove(_character.Coordinate.Move(input.Direction)))
        {
            if (_character.Direction != input.Direction)
            {
                _character.Direction = input.Direction;
                _character.ChangeState(new CharacterStandState(_character, _state));
                _character.Connection.WriteAndFlush(new TurnInput(_character.Direction));
            }
            return;
        }
        if (_state == PlayerState.Idle)
            _character.ChangeState(CharacterMoveState.Move(_character, input));
        else if (_state == PlayerState.FightStand)
            _character.ChangeState(CharacterMoveState.FightWalk(_character, input));
    }

    public static CharacterStandState Idle(ICharacter character)
    {
        return new CharacterStandState(character, PlayerState.Idle);
    }

    public static CharacterStandState FightStand(ICharacter character)
    {
        return new CharacterStandState(character, PlayerState.FightStand);
    }
}