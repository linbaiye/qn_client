using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Godot;
using QnClient.code.sprite;
using QnClient.code.util;

namespace QnClient.code.entity;

public partial class MonsterAnimationPlayer : AnimationPlayer
{
    
    private static readonly ZipFileSpriteLoader SpriteLoader = ZipFileSpriteLoader.Instance;

    private static readonly AtdLoader AtdLoader = AtdLoader.Instance;
    
    private static readonly Dictionary<string, NpcState> MonsterActionMap = new()
    {
        { "MOVE", NpcState.Move },
        { "TURNNING", NpcState.Idle },
        { "STRUCTED", NpcState.Hurt },
        {  "HIT1", NpcState.Attack },
        {  "DIE", NpcState.Die },
        {  "TURN", NpcState.Turn },
    };
    
    private static readonly Dictionary<string, CreatureDirection> DirectionMap = new ()
    {
        { "DR_0", CreatureDirection.Up },
        { "DR_1", CreatureDirection.UpRight },
        { "DR_2", CreatureDirection.Right },
        { "DR_3", CreatureDirection.DownRight },
        { "DR_4", CreatureDirection.Down },
        { "DR_5", CreatureDirection.DownLeft },
        { "DR_6", CreatureDirection.Left },
        { "DR_7", CreatureDirection.UpLeft },
    };


    private void CreateAnimation(AnimationDescriptor descriptor, OffsetTexture[] sprites, Animation.LoopModeEnum loopModeEnum, OffsetTexture[]? effect = null)
    {
        var animationLibrary = new AnimationLibrary();
        if (HasAnimationLibrary(descriptor.State.ToString()))
        {
            animationLibrary = GetAnimationLibrary(descriptor.State.ToString());
        }
        else
        {
            AddAnimationLibrary(descriptor.State.ToString(), animationLibrary);
        }
        Animation animation = new Animation();
        if (animationLibrary.HasAnimation(descriptor.Direction.ToString()))
        {
            animation = animationLibrary.GetAnimation(descriptor.Direction.ToString());
        }
        else
        {
            animationLibrary.AddAnimation(descriptor.Direction.ToString(), animation);
        }
        var empty = new Texture();
        float step = (float)(descriptor.TickPerFrame * 10) / 1000;
        animation.Length = step * descriptor.FrameIndices.Length;
        animation.LoopMode = loopModeEnum;
        var bodyTextureIdx = animation.AddTrack(Animation.TrackType.Value);
        var bodyOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
        var attackEffectTextureIdx = animation.AddTrack(Animation.TrackType.Value);
        var attackEffectOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
        var mouseAreaPosition = animation.AddTrack(Animation.TrackType.Value);
        var mouseAreaSize = animation.AddTrack(Animation.TrackType.Value);
        animation.TrackSetPath(bodyTextureIdx, "Body:texture");
        animation.TrackSetPath(bodyOffsetIdx, "Body:offset");
        animation.TrackSetPath(attackEffectTextureIdx, "AttackEffect:texture");
        animation.TrackSetPath(attackEffectOffsetIdx, "AttackEffect:offset");
        animation.TrackSetPath(mouseAreaPosition, "Body/MouseArea:position");
        animation.TrackSetPath(mouseAreaSize, "Body/MouseArea:size");
        animation.ValueTrackSetUpdateMode(bodyTextureIdx, Animation.UpdateMode.Discrete);
        animation.ValueTrackSetUpdateMode(bodyOffsetIdx, Animation.UpdateMode.Discrete);
        animation.ValueTrackSetUpdateMode(attackEffectTextureIdx, Animation.UpdateMode.Discrete);
        animation.ValueTrackSetUpdateMode(attackEffectOffsetIdx, Animation.UpdateMode.Discrete);
        animation.ValueTrackSetUpdateMode(mouseAreaPosition, Animation.UpdateMode.Discrete);
        animation.ValueTrackSetUpdateMode(mouseAreaSize, Animation.UpdateMode.Discrete);
        float time = 0;
        foreach (var frameIndex in descriptor.FrameIndices)
        {
            var textureOffset = sprites[frameIndex].Offset + VectorUtil.DefaultTextureOffset;
            animation.TrackInsertKey(bodyTextureIdx, time, sprites[frameIndex].Texture);
            animation.TrackInsertKey(bodyOffsetIdx, time, textureOffset);
            animation.TrackInsertKey(mouseAreaPosition, time, textureOffset);
            animation.TrackInsertKey(mouseAreaSize, time, sprites[frameIndex].OriginalSize);
            if (effect != null)
            {
                animation.TrackInsertKey(attackEffectTextureIdx, time, effect[frameIndex].Texture);
                animation.TrackInsertKey(attackEffectOffsetIdx, time, effect[frameIndex].Offset + VectorUtil.DefaultTextureOffset);
            }
            else
            {
                animation.TrackInsertKey(attackEffectTextureIdx, time, empty);
                animation.TrackInsertKey(attackEffectOffsetIdx, time, Vector2.Zero);
            }
            time += step;
        }
    }
    
    private class AnimationDescriptor(
        NpcState state,
        CreatureDirection direction,
        int[] frameIndices,
        int tickPerFrame)
    {
        public NpcState State { get; } = state;

        public CreatureDirection Direction { get; } = direction;

        public int[] FrameIndices { get; } = frameIndices;
        public int TickPerFrame { get; } = tickPerFrame;
    }
    
        
    private static int[] ParseFrameIndices(int total, string[] strings)
    {
        List<int> result = new();
        for (int i = 'A', index = 5, j = 0; i <= 'Z' && index + 2 < strings.Length && j < total; i++, index += 3, j++)
        {
            var frame = strings[index];
            if (string.IsNullOrEmpty(frame))
            {
                continue;
            }
            result.Add(frame.ToInt());
        }
        return result.ToArray();
    }


    private List<AnimationDescriptor> LoadAnimationDescriptors(string atd)
    {
        var atdStrings = AtdLoader.Load(atd);
        List<AnimationDescriptor> result = new();
        foreach (var atdString in atdStrings)
        {
            var tokens = Regex.Replace(atdString, @"\s+", "").Split(",");
            if (string.IsNullOrEmpty(tokens[0]) || "Name".Equals(tokens[0]))
            {
                continue;
            }
            string action = tokens[1];
            int frameTime = tokens[4].ToInt();
            if (MonsterActionMap.TryGetValue(action, out var state) && DirectionMap.TryGetValue(tokens[2], out var dire)) 
                result.Add(new AnimationDescriptor(state, dire, ParseFrameIndices(tokens[3].ToInt(), tokens), frameTime));
        }
        return result;
    }

    public float MoveAnimationLength => GetAnimation(NpcState.Move + "/" + CreatureDirection.Up).Length;

    public void Initialize(string spriteName, string atd)
    {
        var sprites = SpriteLoader.Load(spriteName);
        OffsetTexture[]? effect = SpriteLoader.Exists(spriteName + "m")
            ? SpriteLoader.Load(spriteName + "m")
            : null;
        var animationDescriptors = LoadAnimationDescriptors(atd);
        foreach (var animationDescriptor in animationDescriptors)
        {
            if (animationDescriptor.State == NpcState.Idle)
                CreateAnimation(animationDescriptor, sprites, Animation.LoopModeEnum.Pingpong, effect);
            else
                CreateAnimation(animationDescriptor, sprites, Animation.LoopModeEnum.None, effect);
        }
    }
    
    public void Play(NpcState state, CreatureDirection direction, int fromMillis = 0)
    {
        Stop();
        Play(state + "/" + direction, (float)fromMillis / 1000);
    }
    
    public void PlayIdle(CreatureDirection direction, int fromMillis = 0)
    {
        Play(NpcState.Idle, direction, fromMillis);
    }
    
    public void PlayMove(CreatureDirection direction, int fromMillis = 0)
    {
        Play(NpcState.Move, direction, fromMillis);
    }
}