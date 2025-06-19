using Godot;

namespace QnClient.code.player;

public interface ICharacterState
{
    void Move(MoveInput input);
}