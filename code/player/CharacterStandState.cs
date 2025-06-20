using QnClient.code.creature;

namespace QnClient.code.player;

public class CharacterStandState : ICharacterState
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
    
    public void Move(MoveInput input)
    {
        if (_state == PlayerState.Idle)
            _character.ChangeState(CharacterMoveState.Move(_character, input));
        else if (_state == PlayerState.FightStand)
            _character.ChangeState(CharacterMoveState.FightWalk(_character, input));
    }

    public void PhysicProcess(double deltaSeconds)
    {
    }
    
    public void Process(double deltaSeconds)
    {
        
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