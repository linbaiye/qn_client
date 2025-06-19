using Godot;
using QnClient.code.creature;

namespace QnClient.code.player;

public partial class Character : Player, ICharacter
{
    private ICharacterState _characterState;

    public void ChangeState(ICharacterState state)
    {
        throw new System.NotImplementedException();
    }

    public float GetAnimationLength(string action)  => AnimationPlayer.GetAnimation(action + "/" + CreatureDirection.Up).Length;
}