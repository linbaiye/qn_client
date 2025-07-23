using Godot;
using QnClient.code.message;

namespace QnClient.code.entity;

public partial class DynamicObject : AbstractEntity
{
    public override void _Ready()
    {
        base._Ready();
    }

    public override void HandleEntityMessage(IEntityMessage message)
    {
        throw new System.NotImplementedException();
    }

    public override bool IsCoveringPosition(Vector2 position)
    {
        throw new System.NotImplementedException();
    }
}