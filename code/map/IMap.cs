using System.Numerics;
using Godot;
using QnClient.code.entity;

namespace QnClient.code.map;

public interface IMap
{
    bool CanMove(Vector2I coordinate);

    void Occupy(IEntity entity);
    
    void Free(IEntity entity);
    
    string Name { get; }
}