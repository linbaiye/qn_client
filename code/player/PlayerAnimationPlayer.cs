using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using NLog;
using QnClient.code.creature;
using QnClient.code.sprite;

namespace QnClient.code.player;

public partial class PlayerAnimationPlayer : AnimationPlayer
{
    private ZipFileSpriteLoader _spriteLoader;
        
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private static readonly float WalkTick = 0.14f;
    
    private static readonly int WalkSpriteNumber = 6;

    public override void _Ready()
    {
        _spriteLoader = ZipFileSpriteLoader.Instance;
        InitializeAnimations();
    }

    private AnimationLibrary CreateAnimationLibrary(int spritesPerDirection,
        float step,
        Sprite[] sprites,
        Animation.LoopModeEnum loopModeEnum = Animation.LoopModeEnum.None)
    {
        int start = 0;
        AnimationLibrary animationLibrary = new AnimationLibrary();
        var empty = new Texture();
        foreach (var dir in Enum.GetValues(typeof(CreatureDirection)))
        {
            Animation animation = new Animation();
            animation.Length = step * spritesPerDirection;
            animation.LoopMode = loopModeEnum;
            var bodyTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            var bodyOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            var legTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            var legOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            var bootTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            var bootOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            var leftWristTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            var leftWristOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            var rightWristTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            var rightWristOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            var chestTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            var chestOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            var armorTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            var armorOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            var hairTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            var hairOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            var hatTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            var hatOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            var weaponTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            var weaponOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            var attackEffectTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            var attackEffectOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            animation.TrackSetPath(bodyTextureIdx, "Body:texture");
            animation.TrackSetPath(bodyOffsetIdx, "Body:offset");
            animation.TrackSetPath(legTextureIdx, "Leg:texture");
            animation.TrackSetPath(legOffsetIdx, "Leg:offset");
            animation.TrackSetPath(bootTextureIdx, "Boot:texture");
            animation.TrackSetPath(bootOffsetIdx, "Boot:offset");
            animation.TrackSetPath(leftWristTextureIdx, "LeftWrist:texture");
            animation.TrackSetPath(leftWristOffsetIdx, "LeftWrist:offset");
            animation.TrackSetPath(rightWristTextureIdx, "RightWrist:texture");
            animation.TrackSetPath(rightWristOffsetIdx, "RightWrist:offset");
            animation.TrackSetPath(chestTextureIdx, "Chest:texture");
            animation.TrackSetPath(chestOffsetIdx, "Chest:offset");
            animation.TrackSetPath(armorTextureIdx, "Armor:texture");
            animation.TrackSetPath(armorOffsetIdx, "Armor:offset");
            animation.TrackSetPath(hairTextureIdx, "Hair:texture");
            animation.TrackSetPath(hairOffsetIdx, "Hair:offset");
            animation.TrackSetPath(hatTextureIdx, "Hat:texture");
            animation.TrackSetPath(hatOffsetIdx, "Hat:offset");
            animation.TrackSetPath(weaponTextureIdx, "Weapon:texture");
            animation.TrackSetPath(weaponOffsetIdx, "Weapon:offset");
            animation.TrackSetPath(attackEffectTextureIdx, "AttackEffect:texture");
            animation.TrackSetPath(attackEffectOffsetIdx, "AttackEffect:offset");
            animation.ValueTrackSetUpdateMode(bodyOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(bodyOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(legTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(legOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(bootTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(bootOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(leftWristTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(leftWristOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(rightWristTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(rightWristOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(chestTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(chestOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(armorTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(armorOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(hairTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(hairOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(hatOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(hatTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(weaponOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(weaponTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(attackEffectTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(attackEffectOffsetIdx, Animation.UpdateMode.Discrete);
            for (int i = 0; i < spritesPerDirection; i++)
            {
                animation.TrackInsertKey(bodyTextureIdx, step * i, sprites[start + i].Texture);
                animation.TrackInsertKey(bodyOffsetIdx, step * i, sprites[start + i].Offset);
                animation.TrackInsertKey(legTextureIdx, step * i, empty);
                animation.TrackInsertKey(legOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(bootTextureIdx, step * i, empty);
                animation.TrackInsertKey(bootOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(leftWristTextureIdx, step * i, empty);
                animation.TrackInsertKey(leftWristOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(rightWristTextureIdx, step * i, empty);
                animation.TrackInsertKey(rightWristOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(chestTextureIdx, step * i, empty);
                animation.TrackInsertKey(chestOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(armorTextureIdx, step * i, empty);
                animation.TrackInsertKey(armorOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(hairTextureIdx, step * i, empty);
                animation.TrackInsertKey(hairOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(hatTextureIdx, step * i, empty);
                animation.TrackInsertKey(hatOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(weaponTextureIdx, step * i, empty);
                animation.TrackInsertKey(weaponOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(attackEffectTextureIdx, step * i, empty);
                animation.TrackInsertKey(attackEffectOffsetIdx, step * i, Vector2.Zero);
            }
            animationLibrary.AddAnimation(dir.ToString(), animation);
            start += spritesPerDirection;
        }
        return animationLibrary;
    }

    // private AnimationLibrary CreateAnimationLibrary(int spriteStart, int spritesPerDirection, float step,
    //     Vector2[] offsets, Animation.LoopModeEnum loopModeEnum = Animation.LoopModeEnum.None, string subdir = "N00")
    // {
    //     int start = spriteStart;
    //     AnimationLibrary animationLibrary = new AnimationLibrary();
    //     var empty = new Texture();
    //     foreach (var dir in Enum.GetValues(typeof(TextServer.Direction)))
    //     {
    //         Animation animation = new Animation();
    //         animation.Length = step * spritesPerDirection;
    //         animation.LoopMode = loopModeEnum;
    //         var textureIdx = animation.AddTrack(Animation.TrackType.Value);
    //         var offsetIdx = animation.AddTrack(Animation.TrackType.Value);
    //         var weaponTextureIdx = animation.AddTrack(Animation.TrackType.Value);
    //         var weaponOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
    //         var hatTextureIdx = animation.AddTrack(Animation.TrackType.Value);
    //         var hatOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
    //         animation.TrackSetPath(textureIdx, "Body:texture");
    //         animation.TrackSetPath(offsetIdx, "Body:offset");
    //         animation.TrackSetPath(weaponTextureIdx, "Weapon:texture");
    //         animation.TrackSetPath(weaponOffsetIdx, "Weapon:offset");
    //         animation.TrackSetPath(hatTextureIdx, "Hat:texture");
    //         animation.TrackSetPath(hatOffsetIdx, "Hat:offset");
    //         animation.ValueTrackSetUpdateMode(offsetIdx, Animation.UpdateMode.Discrete);
    //         animation.ValueTrackSetUpdateMode(offsetIdx, Animation.UpdateMode.Discrete);
    //         animation.ValueTrackSetUpdateMode(weaponOffsetIdx, Animation.UpdateMode.Discrete);
    //         animation.ValueTrackSetUpdateMode(weaponTextureIdx, Animation.UpdateMode.Discrete);
    //         animation.ValueTrackSetUpdateMode(hatOffsetIdx, Animation.UpdateMode.Discrete);
    //         animation.ValueTrackSetUpdateMode(hatTextureIdx, Animation.UpdateMode.Discrete);
    //         for (int i = 0; i < spritesPerDirection; i++)
    //         {
    //             animation.TrackInsertKey(weaponOffsetIdx, step * i, Vector2.Zero);
    //             animation.TrackInsertKey(weaponTextureIdx, step * i, empty);
    //             animation.TrackInsertKey(hatOffsetIdx, step * i, Vector2.Zero);
    //             animation.TrackInsertKey(hatTextureIdx, step * i, empty);
    //             animation.TrackInsertKey(offsetIdx, step * i, offsets[start + i]);
    //             var spriteIndex = start + i;
    //             animation.TrackInsertKey(textureIdx, step * i,
    //                 ResourceLoader.Load<Texture2D>($"res://char/{subdir}/{spriteIndex:D6}.png"));
    //         }
    //         animationLibrary.AddAnimation(dir.ToString(), animation);
    //         start += spritesPerDirection;
    //     }
    //     return animationLibrary;
    // }


    // private static readonly string DIR_PATH = "char/";
    //
    // private AnimationLibrary CreateWalkAnimations(Vector2[] offsets)
    // {
    //     return CreateAnimationLibrary(0, WalkSpriteNumber, WalkTick, offsets);
    // }
    //
    // private AnimationLibrary CreateWalkAnimations(Vector2[] offsets, Texture2D[] texture2Ds)
    // {
    //     return CreateAnimationLibrary(WalkSpriteNumber,  WalkTick, offsets, texture2Ds);
    // }
    //
    
    // private AnimationLibrary CreateStopWalkAnimations(Vector2[] offsets, Texture2D[] texture2Ds)
    // {
    //     Vector2[] off = new Vector2[16];
    //     Texture2D[] tex  = new Texture2D[16];
    //     int walk = 0;
    //     int stop = 48;
    //     int index = 0;
    //     for (int i = 0; i < 8; i++)
    //     {
    //         off[index] = offsets[walk];
    //         tex[index++] = texture2Ds[walk];
    //         // off[index] = offsets[stop];
    //         // tex[index++] = texture2Ds[stop];
    //         walk += 6;
    //         stop += 3;
    //     }
    //     return CreateAnimationLibrary(1,  WalkTick, off, tex);
    // }
    
    // private AnimationLibrary CreateIdleAnimations(Vector2[] offsets)
    // {
    //     return CreateAnimationLibrary(48, 3, 0.1f, offsets, Animation.LoopModeEnum.Linear);
    // }
    //
    // private AnimationLibrary CreateSwordAttackAnimations(Vector2[] offsets)
    // {
    //     return CreateAnimationLibrary(0, 9, 0.07f, offsets, Animation.LoopModeEnum.None, "N02");
    // }
    //
    // private AnimationLibrary CreateAxeAnimations(Vector2[] offsets)
    // {
    //     return CreateAnimationLibrary(0, 8, 0.1f, offsets, Animation.LoopModeEnum.None, "N03");
    // }
    //
    //
    // private AnimationLibrary CreateSwordHardAttackAnimations(Vector2[] offsets)
    // {
    //     return CreateAnimationLibrary(144, 10, 0.1f, offsets, Animation.LoopModeEnum.None, "N02");
    // }

    public float WalkAnimationLength => WalkSpriteNumber * WalkTick;
    public float StopWalkLength =>  WalkTick;

    private static readonly float RunSpeed = 1.4f;
    public float RunAnimationLength => WalkAnimationLength / RunSpeed;

    private void InitializeAnimations()
    {
        var sprites = _spriteLoader.Load("N00");
        AddAnimationLibrary(MoveAction.Walk.ToString(), CreateAnimationLibrary(WalkSpriteNumber, WalkTick, sprites));
        // AddAnimationLibrary(WebSocketPeer.State.StopMove.ToString(), CreateStopWalkAnimations(n02Offsets, n020Textures));
        // AddAnimationLibrary(WebSocketPeer.State.Idle.ToString(), CreateIdleAnimations(n02Offsets));
        // var n00Offsets = LoadOffsets("N02");
        // AddAnimationLibrary(PlayerAttackAction.Sword.ToString(), CreateSwordAttackAnimations(n00Offsets));
        // AddAnimationLibrary(PlayerAttackAction.Sword2H.ToString(), CreateSwordHardAttackAnimations(n00Offsets));
        // var n003ffsets = LoadOffsets("N03");
        // AddAnimationLibrary(PlayerAttackAction.Axe.ToString(), CreateAxeAnimations(n003ffsets));
    }

    // public void SetAxeAnimation()
    // {
    //     var offsets = LoadOffsets("w130");
    //     Dictionary<WebSocketPeer.State, int> stateSpriteStart = new Dictionary<WebSocketPeer.State, int>()
    //     {
    //         { WebSocketPeer.State.Move, 0 },
    //         { WebSocketPeer.State.Idle, 48 },
    //     };
    //     Dictionary<PlayerAttackAction, int> attackAction = new Dictionary<PlayerAttackAction, int>()
    //     {
    //         { PlayerAttackAction.Axe, 0 },
    //     };
    //     foreach (var state in stateSpriteStart.Keys)
    //     {
    //         stateSpriteStart.TryGetValue(state, out var spriteIndex);
    //         foreach (var dir in Enum.GetValues(typeof(TextServer.Direction)))
    //         {
    //             var animation = GetAnimation(state+ "/" + dir);
    //             int count = animation.TrackGetKeyCount(2);
    //             for (int i = 0; i < count; i++)
    //             {
    //                 animation.TrackSetKeyValue(2, i,
    //                     ResourceLoader.Load<Texture2D>($"res://char/w130/{spriteIndex:D6}.png"));
    //                 animation.TrackSetKeyValue(3, i, offsets[spriteIndex]);
    //                 spriteIndex++;
    //             }
    //         }
    //     }
    //     offsets = LoadOffsets("w133");
    //     foreach (var action in attackAction.Keys)
    //     {
    //         attackAction.TryGetValue(action, out var spriteIndex);
    //         foreach (var dir in Enum.GetValues(typeof(TextServer.Direction)))
    //         {
    //             var animation = GetAnimation(action+ "/" + dir);
    //             int count = animation.TrackGetKeyCount(2);
    //             for (int i = 0; i < count; i++)
    //             {
    //                 animation.TrackSetKeyValue(2, i,
    //                     ResourceLoader.Load<Texture2D>($"res://char/w133/{spriteIndex:D6}.png"));
    //                 animation.TrackSetKeyValue(3, i, offsets[spriteIndex]);
    //                 spriteIndex++;
    //             }
    //         }
    //     }
    // }

    // public void HideHatAnimation()
    // {
    //     WebSocketPeer.State[] states = [WebSocketPeer.State.Idle, WebSocketPeer.State.Move];
    //     Texture empty = new Texture();
    //     foreach (var state in states)
    //     {
    //         foreach (var dir in Enum.GetValues(typeof(TextServer.Direction)))
    //         {
    //             var animation = GetAnimation(state+ "/" + dir);
    //             int count = animation.TrackGetKeyCount(0);
    //             for (int i = 0; i < count; i++)
    //             {
    //                 animation.TrackSetKeyValue(4, i, empty);
    //                 animation.TrackSetKeyValue(5, i, Vector2.Zero);
    //             }
    //         }
    //     }
    // }

    // public void SetHatAnimation()
    // {
    //     var offsets = LoadOffsets("v160");
    //     Dictionary<WebSocketPeer.State, int> stateSpriteStart = new Dictionary<WebSocketPeer.State, int>()
    //     {
    //         { WebSocketPeer.State.Move, 0 },
    //         { WebSocketPeer.State.Idle, 48 },
    //     };
    //     
    //     foreach (var state in stateSpriteStart.Keys)
    //     {
    //         stateSpriteStart.TryGetValue(state, out var spriteIndex);
    //         foreach (var dir in Enum.GetValues(typeof(TextServer.Direction)))
    //         {
    //             var animation = GetAnimation(state+ "/" + dir);
    //             int count = animation.TrackGetKeyCount(0);
    //             for (int i = 0; i < count; i++)
    //             {
    //                 animation.TrackSetKeyValue(4, i,
    //                     ResourceLoader.Load<Texture2D>($"res://char/v160/{spriteIndex:D6}.png"));
    //                 animation.TrackSetKeyValue(5, i, offsets[spriteIndex]);
    //                 spriteIndex++;
    //             }
    //         }
    //     }
    // }
    
    // public void SetSwordAnimation()
    // {
    //     var offsets = LoadOffsets("w10");
    //     Dictionary<string, int> stateSpriteStart = new Dictionary<string, int>()
    //     {
    //         { WebSocketPeer.State.Move.ToString(), 0 },
    //         { WebSocketPeer.State.Idle.ToString(), 48 },
    //     };
    //     Dictionary<PlayerAttackAction, int> swordStateSpriteStart = new Dictionary<PlayerAttackAction, int>()
    //     {
    //         { PlayerAttackAction.Sword, 0 },
    //         { PlayerAttackAction.Sword2H, 144 },
    //     };
    //     foreach (var state in stateSpriteStart.Keys)
    //     {
    //         stateSpriteStart.TryGetValue(state, out var spriteIndex);
    //         foreach (var dir in Enum.GetValues(typeof(TextServer.Direction)))
    //         {
    //             var animation = GetAnimation(state+ "/" + dir);
    //             int count = animation.TrackGetKeyCount(2);
    //             for (int i = 0; i < count; i++)
    //             {
    //                 animation.TrackSetKeyValue(2, i,
    //                     ResourceLoader.Load<Texture2D>($"res://char/w10/{spriteIndex:D6}.png"));
    //                 animation.TrackSetKeyValue(3, i, offsets[spriteIndex]);
    //                 spriteIndex++;
    //             }
    //         }
    //     }
    //     offsets = LoadOffsets("w12");
    //     foreach (var state in swordStateSpriteStart.Keys)
    //     {
    //         swordStateSpriteStart.TryGetValue(state, out var spriteIndex);
    //         foreach (var dir in Enum.GetValues(typeof(TextServer.Direction)))
    //         {
    //             var animation = GetAnimation(state+ "/" + dir);
    //             int count = animation.TrackGetKeyCount(2);
    //             for (int i = 0; i < count; i++)
    //             {
    //                 animation.TrackSetKeyValue(2, i,
    //                     ResourceLoader.Load<Texture2D>($"res://char/w12/{spriteIndex:D6}.png"));
    //                 animation.TrackSetKeyValue(3, i, offsets[spriteIndex]);
    //                 spriteIndex++;
    //             }
    //         }
    //     }
    // }

    // public void PlayIdleAnimation(TextServer.Direction direction)
    // {
    //     Play(WebSocketPeer.State.Idle + "/" + direction, -1, 0.16666f);
    // }
    //
    // public void PlayFlyAnimation(TextServer.Direction direction)
    // {
    //     Play(WebSocketPeer.State.Idle + "/" + direction);
    // }
    //
    // public void PlayWalkAnimation(TextServer.Direction direction)
    // {
    //     Play(WebSocketPeer.State.Move + "/" + direction);
    // }
    //
    //
    // public void PlayStopWalkAnimation(TextServer.Direction direction)
    // {
    //     Play(WebSocketPeer.State.StopMove + "/" + direction);
    // }
    //
    // public void PlayRunAnimation(TextServer.Direction direction)
    // {
    //     Play(WebSocketPeer.State.Move + "/" + direction, -1, RunSpeed);
    // }
    //
    // public void PlayAnimation(WebSocketPeer.State state, TextServer.Direction direction)
    // {
    //     Play(state + "/" + direction);
    // }
    //
    // public void PlayAnimation(PlayerAttackAction playerAttackAction, TextServer.Direction direction)
    // {
    //     Play(playerAttackAction + "/" + direction);
    // }
    
    
}