using Godot;
using QnClient.code.entity;

namespace QnClient.code.map;

public interface IMap
{
    bool CanMove(Vector2I coordinate);

	public Vector2I MapSize { get; }
    
    Vector2I Start { get; }
    
    Vector2I End { get; }
    
    public string Name { get; }

    void HandleEntityEvent(IEntityEvent entityEvent);

    void Load(string name, string textureResourceName, Vector2I coordinate);

}