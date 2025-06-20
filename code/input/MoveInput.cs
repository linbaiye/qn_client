using Godot;
using QnClient.code.creature;

namespace QnClient.code.player;

public readonly struct MoveInput(CreatureDirection direction)
{
    public CreatureDirection Direction { get; } = direction;
}