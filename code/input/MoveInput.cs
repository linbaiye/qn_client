using QnClient.code.creature;

namespace QnClient.code.input;

public readonly struct MoveInput(CreatureDirection direction)
{
    public CreatureDirection Direction { get; } = direction;
}