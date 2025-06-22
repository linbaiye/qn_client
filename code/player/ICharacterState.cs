using Godot;
using QnClient.code.input;

namespace QnClient.code.player;

public interface ICharacterState
{
    void Move(MoveInput input);

    void PhysicProcess(double delta);
    
    void Process(double delta);
}