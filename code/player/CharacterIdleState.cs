using QnClient.code.creature;

namespace QnClient.code.player;

public class CharacterIdleState(ICharacter character) : ICharacterState
{
    private readonly ICharacter _character = character;
    private readonly double _stateSeconds = character.GetAnimationLength(CreatureState.Idle.ToString());
    private double _elapsedSeconds = 0;


    public void Move(MoveInput input)
    {
        _character.ChangeState();
    }

    public void PhysicUpdate(double deltaSeconds)
    {
        _elapsedSeconds += deltaSeconds;
        if (_elapsedSeconds >= _stateSeconds)
            _elapsedSeconds = 0;
    }

    public void Update(double deltaSeconds)
    {
        
    }
}