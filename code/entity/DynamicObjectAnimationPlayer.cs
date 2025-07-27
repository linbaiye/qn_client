using System.Collections.Generic;
using Godot;
using QnClient.code.message;
using QnClient.code.sprite;
using QnClient.code.util;

namespace QnClient.code.entity;

public partial class DynamicObjectAnimationPlayer : AnimationPlayer
{

    private static readonly float Step = 0.2f;

    private OffsetTexture[] _sprites;


    private void Create(Animation animation, Animation.LoopModeEnum loopModeEnum, int start, int end)
    {
        animation.LoopMode = loopModeEnum;
        var bodyTexture = animation.AddTrack(Animation.TrackType.Value);
        var bodyOffset = animation.AddTrack(Animation.TrackType.Value);
        var mouseAreaPosition = animation.AddTrack(Animation.TrackType.Value);
        var areaSize = animation.AddTrack(Animation.TrackType.Value);
        animation.TrackSetPath(bodyTexture, "Body:texture");
        animation.TrackSetPath(bodyOffset, "Body:offset");
        animation.TrackSetPath(mouseAreaPosition, "Body/MouseArea:position");
        animation.TrackSetPath(areaSize, "Body/MouseArea:size");
        animation.ValueTrackSetUpdateMode(bodyTexture, Animation.UpdateMode.Discrete);
        animation.ValueTrackSetUpdateMode(bodyOffset, Animation.UpdateMode.Discrete);
        animation.ValueTrackSetUpdateMode(mouseAreaPosition, Animation.UpdateMode.Discrete);
        animation.ValueTrackSetUpdateMode(areaSize, Animation.UpdateMode.Discrete);
        for (int i = start, t = 0; i <= end; i++, t++)
        {
            var textureOffset = _sprites[i].Offset + VectorUtil.DefaultTextureOffset;
            var time = Step * t;
            animation.TrackInsertKey(bodyTexture, time, _sprites[i].Texture);
            animation.TrackInsertKey(bodyOffset, time, textureOffset);
            animation.TrackInsertKey(mouseAreaPosition, time, textureOffset);
            animation.TrackInsertKey(areaSize, time, _sprites[i].OriginalSize);
        }
    }
    
    public OffsetTexture Initialize(string sprite, List<DynamicObjectSnapshot.Animate> animates)
    {
        _sprites = ZipFileSpriteLoader.Instance.Load(sprite);
        var library = new AnimationLibrary();
        foreach (var ani in animates)
        {
            var animation = new Animation();
            Create(animation, ani.Loop ? Animation.LoopModeEnum.Linear: Animation.LoopModeEnum.None, ani.Start, ani.End);
            library.AddAnimation(ani.Id.ToString(), animation);
        }
        AddAnimationLibrary("default", library);
        return _sprites[0];
    }

    public void PlayId(int n, int elapsedMillis = 0)
    {
        PlaySection("default/" + n, (float) elapsedMillis / 1000);
    }

    public void Play(int start, int end, int elapsed = 0, bool loop = false)
    {
        Stop();
        float trueStart = start == end ? start * Step : (float)elapsed / 1000 + start * Step;
        if (!loop)
        {
            float endSec = start == end ? (end + 1) * Step - 0.01f : end * Step;
            PlaySection("default/default", trueStart, endSec);
        }
        else
        {
            var animationLibrary = GetAnimationLibrary("default");
            if (animationLibrary.HasAnimation("loop"))
                 animationLibrary.RemoveAnimation("loop");
            var animation = new Animation();
            Create(animation, Animation.LoopModeEnum.Linear, start, end + 1);
            animationLibrary.AddAnimation("loop", animation);
            Play("default/loop");
        }
    }
}