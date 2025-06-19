namespace QnClient.code.player;

public class CharacterMoveState : ICharacterState
{
    private readonly ICharacter _character;
    private readonly double _stateSeconds;
    private readonly double _elapsedSeconds;
    private readonly MoveAction _action;

    public CharacterMoveState(ICharacter character, MoveAction action)
    {
        _character = character;
        _action = action;
        _stateSeconds = character.GetAnimationLength()
    }

    public void Move(MoveInput input)
    {
        throw new System.NotImplementedException();
    }
}