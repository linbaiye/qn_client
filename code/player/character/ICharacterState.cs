using QnClient.code.input;

namespace QnClient.code.player.character;

public interface ICharacterState
{
    void Move(MoveInput input);

    void PhysicProcess(double delta);
    
}