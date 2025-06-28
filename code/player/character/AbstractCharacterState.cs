using QnClient.code.input;

namespace QnClient.code.player.character;

public abstract class AbstractCharacterState : ICharacterState
{
    public virtual void Move(MoveInput input)
    {
    }

    public virtual void PhysicProcess(double delta)
    {
    }

}