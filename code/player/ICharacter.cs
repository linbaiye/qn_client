using Godot;
using QnClient.code.creature;
using QnClient.code.player.kungfu;

namespace QnClient.code.player;

public interface ICharacter
{
    void ChangeState(ICharacterState state);

    PlayerAnimationPlayer AnimationPlayer
    {
        get;
    }
    
    CreatureDirection Direction { get; set; }
    
    Vector2 Position { get; set; }

    bool MovePressed { get; }

    Vector2 GetLocalMousePosition();
    
    FootKungFu? FootKungFu { get; }
}