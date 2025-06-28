using Godot;
using QnClient.code.util;

namespace QnClient.code.entity;

public class EntityMover
{
    private readonly Node2D _entity;

    private readonly float _durationSeconds;

    private float _elapsedSeconds;

    private readonly Vector2 _velocity;

    public EntityMover(Node2D entity, float durationSeconds, Vector2 velocity, float elapsedSeconds = 0)
    {
        _entity = entity;
        _durationSeconds = durationSeconds;
        _elapsedSeconds = elapsedSeconds;
        _velocity = velocity;
        _entity.Position += _velocity * elapsedSeconds;
    }
    
    /// <summary>
    /// Return true if done moving, false else.
    /// </summary>
    /// <param name="delta"></param>
    /// <returns></returns>
    public bool PhysicProcess(double delta)
    {
        if (_elapsedSeconds >= _durationSeconds)
            return true;
        _entity.Position += _velocity * (float)delta;
        _elapsedSeconds += (float)delta;
        if (_elapsedSeconds < _durationSeconds)
            return false;
        _entity.Position = _entity.Position.Snapped(VectorUtil.TileSize);
        return true;
    }
}