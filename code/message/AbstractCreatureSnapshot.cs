using Godot;
using QnClient.code.entity;

namespace QnClient.code.message;

public abstract class AbstractCreatureSnapshot
{
    public string Name { get; protected init; }
    
    public long Id { get; protected init; }
    
    public Vector2I Coordinate { get; protected init; }
    
    public CreatureDirection Direction { get; protected init; }

}