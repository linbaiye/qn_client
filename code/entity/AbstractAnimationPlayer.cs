using System;
using Godot;

namespace QnClient.code.entity;

public abstract partial class AbstractAnimationPlayer : AnimationPlayer
{
    
    protected int BodyTextureIdx = -1;
    protected int BodyOffsetIdx = -1;
    protected int AreaSize = -1;
    private Vector2[] _areas;
    private Vector2[] _offsets;

    protected CreatureDirection CurrentDirection;

    protected void ExtractIdlePositions()
    {
        var creatureDirections = Enum.GetValues<CreatureDirection>();
        _offsets = new Vector2[creatureDirections.Length];
        _areas = new Vector2[creatureDirections.Length];
        foreach (var direction in creatureDirections)
        {
            var animation = GetAnimation("Idle/" + direction);
            var s = (Vector2)animation.TrackGetKeyValue(AreaSize, 0);
            var o = (Vector2)animation.TrackGetKeyValue(BodyOffsetIdx, 0);
            _areas[(int)direction] = s;
            _offsets[(int)direction] = o;
        }
    }
    public Vector2 BodyOffset => _offsets[(int)CurrentDirection];
    
    public Vector2 BodySize => _areas[(int)CurrentDirection];


    protected void PlayLastFrame(string name)
    {
        var animation = GetAnimation(name);
        var count = animation.TrackGetKeyCount(BodyOffsetIdx);
        var time = animation.TrackGetKeyTime(BodyOffsetIdx, count - 1);
        PlaySection(name, time);
    }
}