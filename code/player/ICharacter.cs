namespace QnClient.code.player;

public interface ICharacter
{
    void ChangeState(ICharacterState state);

    float GetAnimationLength(string action);
    
}