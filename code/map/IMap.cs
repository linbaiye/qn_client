using Godot;

namespace QnClient.code.map;

public interface IMap
{
    bool CanMove(Vector2I coordinate);

	public Vector2I MapSize { get; }
    
    Vector2I Start { get; }
    
    Vector2I End { get; }
    
    public string Name { get; }
    
}