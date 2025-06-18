using System;
using Godot;
using NLog;
using QnClient.code.creature;
using QnClient.code.sprite;

namespace QnClient.code.player;

public partial class PlayerAnimationPlayer : AnimationPlayer
{
    private ZipFileSpriteLoader _spriteLoader;
        
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private const int WalkSpriteNumber = 6;
    private const float WalkStep = 0.14f;
    
    private const int IdleSpriteNumber = 3;
    private const float IdleStep = 0.6f;
    
    private const int FightWalkSpriteNumber = 6;
    private const float FightWalkStep = 0.14f;
    
    private const int FightStandSpriteNumber = 3;
    private const float FightStandStep = 0.6f;
    
    private const int SitSpriteNumber = 5;
    private const float SitStep = 0.15f;
    
    private const int HurtSpriteNumber = 4;
    private const float HurtStep = 0.07f;
    
    private const int DieSpriteNumber = 6;
    private const float DieStep = 0.25f;
    
    private const int HelloSpriteNumber = 3;
    private const float HelloStep = 0.25f;
    
    private const int KickSpriteNumber = 7;
    private const float KickStep = 0.08f;
    
    private const int PunchSpriteNumber = 5;
    private const float PunchStep = 0.08f;
    
    private const int Sword1HSpriteNumber = 9;
    private const float Sword1HStep = 0.08f;
    
    private const int Blade2HSpriteNumber = 9;
    private const float Blade2HStep = 0.07f;
    
    private const int Sword2HSpriteNumber = 10;
    private const float Sword2HStep = 0.08f;
    
    private const int AxeSpriteNumber = 8;
    private const float AxeStep = 0.1f;
    
    private const int BowSpriteNumber = 6;
    private const float BowStep = 0.1f;
    
    private const int ThrowSpriteNumber = 9;
    private const float ThrowStep = 0.1f;
    
    private int _bodyTextureIdx = -1;
    private int _bodyOffsetIdx = -1;
    private int _legTextureIdx = -1;
    private int _legOffsetIdx = -1;
    private int _bootTextureIdx = -1;
    private int _bootOffsetIdx = -1;
    private int _leftWristTextureIdx = -1;
    private int _leftWristOffsetIdx = -1;
    private int _rightWristTextureIdx = -1;
    private int _rightWristOffsetIdx = -1;
    private int _chestTextureIdx = -1;
    private int _chestOffsetIdx = -1;
    private int _armorTextureIdx = -1;
    private int _armorOffsetIdx = -1;
    private int _hairTextureIdx = -1;
    private int _hairOffsetIdx = -1;
    private int _hatTextureIdx = -1;
    private int _hatOffsetIdx = -1;
    private int _weaponTextureIdx = -1;
    private int _weaponOffsetIdx = -1;
    private int _attackEffectTextureIdx = -1;
    private int _attackEffectOffsetIdx = -1;
    
    private const int DirectionNumber = 8;

    public override void _Ready()
    {
        _spriteLoader = ZipFileSpriteLoader.Instance;
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
            _bodyTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            _bodyOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            _legTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            _legOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            _bootTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            _bootOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            _leftWristTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            _leftWristOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            _rightWristTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            _rightWristOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            _chestTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            _chestOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            _armorTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            _armorOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            _hairTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            _hairOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            _hatTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            _hatOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            _weaponTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            _weaponOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            _attackEffectTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            _attackEffectOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            animation.TrackSetPath(_bodyTextureIdx, "Body:texture");
            animation.TrackSetPath(_bodyOffsetIdx, "Body:offset");
            animation.TrackSetPath(_legTextureIdx, "Leg:texture");
            animation.TrackSetPath(_legOffsetIdx, "Leg:offset");
            animation.TrackSetPath(_bootTextureIdx, "Boot:texture");
            animation.TrackSetPath(_bootOffsetIdx, "Boot:offset");
            animation.TrackSetPath(_leftWristTextureIdx, "LeftWrist:texture");
            animation.TrackSetPath(_leftWristOffsetIdx, "LeftWrist:offset");
            animation.TrackSetPath(_rightWristTextureIdx, "RightWrist:texture");
            animation.TrackSetPath(_rightWristOffsetIdx, "RightWrist:offset");
            animation.TrackSetPath(_chestTextureIdx, "Chest:texture");
            animation.TrackSetPath(_chestOffsetIdx, "Chest:offset");
            animation.TrackSetPath(_armorTextureIdx, "Armor:texture");
            animation.TrackSetPath(_armorOffsetIdx, "Armor:offset");
            animation.TrackSetPath(_hairTextureIdx, "Hair:texture");
            animation.TrackSetPath(_hairOffsetIdx, "Hair:offset");
            animation.TrackSetPath(_hatTextureIdx, "Hat:texture");
            animation.TrackSetPath(_hatOffsetIdx, "Hat:offset");
            animation.TrackSetPath(_weaponTextureIdx, "Weapon:texture");
            animation.TrackSetPath(_weaponOffsetIdx, "Weapon:offset");
            animation.TrackSetPath(_attackEffectTextureIdx, "AttackEffect:texture");
            animation.TrackSetPath(_attackEffectOffsetIdx, "AttackEffect:offset");
            animation.ValueTrackSetUpdateMode(_bodyOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_bodyOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_legTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_legOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_bootTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_bootOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_leftWristTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_leftWristOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_rightWristTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_rightWristOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_chestTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_chestOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_armorTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_armorOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_hairTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_hairOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_hatOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_hatTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_weaponOffsetIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_weaponTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_attackEffectTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_attackEffectOffsetIdx, Animation.UpdateMode.Discrete);
            for (int i = 0; i < spritesPerDirection; i++)
            {
                animation.TrackInsertKey(_bodyTextureIdx, step * i, sprites[start + i].Texture);
                animation.TrackInsertKey(_bodyOffsetIdx, step * i, sprites[start + i].Offset);
                animation.TrackInsertKey(_legTextureIdx, step * i, empty);
                animation.TrackInsertKey(_legOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(_bootTextureIdx, step * i, empty);
                animation.TrackInsertKey(_bootOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(_leftWristTextureIdx, step * i, empty);
                animation.TrackInsertKey(_leftWristOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(_rightWristTextureIdx, step * i, empty);
                animation.TrackInsertKey(_rightWristOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(_chestTextureIdx, step * i, empty);
                animation.TrackInsertKey(_chestOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(_armorTextureIdx, step * i, empty);
                animation.TrackInsertKey(_armorOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(_hairTextureIdx, step * i, empty);
                animation.TrackInsertKey(_hairOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(_hatTextureIdx, step * i, empty);
                animation.TrackInsertKey(_hatOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(_weaponTextureIdx, step * i, empty);
                animation.TrackInsertKey(_weaponOffsetIdx, step * i, Vector2.Zero);
                animation.TrackInsertKey(_attackEffectTextureIdx, step * i, empty);
                animation.TrackInsertKey(_attackEffectOffsetIdx, step * i, Vector2.Zero);
            }
            animationLibrary.AddAnimation(dir.ToString(), animation);
            start += spritesPerDirection;
        }
        return animationLibrary;
    }



    public void InitializeAnimations(bool male)
    {
        string prefix = male ? "N0" : "A0";
        var sprites = _spriteLoader.Load(prefix + "0");
        AddAnimationLibrary(MoveAction.Walk.ToString(), CreateAnimationLibrary(WalkSpriteNumber, WalkStep, sprites));
        var tmp = new Sprite[IdleSpriteNumber * DirectionNumber];
        int index = WalkSpriteNumber * DirectionNumber;
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(CreatureState.Idle.ToString(), CreateAnimationLibrary(IdleSpriteNumber, IdleStep, tmp, Animation.LoopModeEnum.Linear));
        index += tmp.Length;
        
        tmp = new Sprite[FightWalkSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(MoveAction.FightWalk.ToString(), CreateAnimationLibrary(FightWalkSpriteNumber, FightWalkStep, tmp));
        index += tmp.Length;
        
        tmp = new Sprite[FightStandSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(CreatureState.FightStand.ToString(), CreateAnimationLibrary(FightStandSpriteNumber, FightStandStep, tmp, Animation.LoopModeEnum.Linear));
        index += tmp.Length;
        
        tmp = new Sprite[SitSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(CreatureState.Sit.ToString(), CreateAnimationLibrary(SitSpriteNumber , SitStep, tmp));
        index += tmp.Length;
        
        tmp = new Sprite[HurtSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(CreatureState.Hurt.ToString(), CreateAnimationLibrary(HurtSpriteNumber, HurtStep, tmp));
        index += tmp.Length;
        
        tmp = new Sprite[DieSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index + DieSpriteNumber * (DirectionNumber - 1), tmp, 0, DieSpriteNumber);
        Array.Copy(sprites, index, tmp, DieSpriteNumber, tmp.Length - DieSpriteNumber);
        AddAnimationLibrary(CreatureState.Die.ToString(), CreateAnimationLibrary(DieSpriteNumber, DieStep, tmp));
        index += tmp.Length;
        
        tmp = new Sprite[HelloSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(CreatureState.Hello.ToString(), CreateAnimationLibrary(HelloSpriteNumber, HelloStep, tmp));
        
        sprites = _spriteLoader.Load(prefix + "1");
        AddAnimationLibrary(AttackAction.Kick.ToString(), CreateAnimationLibrary(KickSpriteNumber, KickStep, sprites));
        index = KickSpriteNumber * DirectionNumber;
        
        tmp = new Sprite[PunchSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(AttackAction.Punch.ToString(), CreateAnimationLibrary(PunchSpriteNumber, PunchStep, tmp));
        
        sprites = _spriteLoader.Load(prefix + "2");
        AddAnimationLibrary(AttackAction.Sword1H.ToString(), CreateAnimationLibrary(Sword1HSpriteNumber, Sword1HStep, sprites));
        AddAnimationLibrary(AttackAction.Throw.ToString(), CreateAnimationLibrary(ThrowSpriteNumber, ThrowStep, sprites));
        index = Sword1HSpriteNumber * DirectionNumber;
        
        tmp = new Sprite[Blade2HSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(AttackAction.Blade2H.ToString(), CreateAnimationLibrary(Blade2HSpriteNumber, Blade2HStep, tmp));
        index += tmp.Length;
        
        tmp = new Sprite[Sword2HSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(AttackAction.Sword2H.ToString(), CreateAnimationLibrary(Sword2HSpriteNumber, Sword2HStep, tmp));

        sprites = _spriteLoader.Load(prefix + "3");
        AddAnimationLibrary(AttackAction.Axe.ToString(), CreateAnimationLibrary(AxeSpriteNumber, AxeStep, sprites));
        
        sprites = _spriteLoader.Load(prefix + "4");
        AddAnimationLibrary(AttackAction.Bow.ToString(), CreateAnimationLibrary(BowSpriteNumber, BowStep, sprites));
        
    }


    public void PlayWalk(CreatureDirection direction)
    {
        Play(MoveAction.Walk + "/" + direction);
    }
    
    public void PlayFightWalk(CreatureDirection direction)
    {
        Play(MoveAction.FightWalk + "/" + direction);
    }
    
    public void PlayFightStand(CreatureDirection direction)
    {
        Play(CreatureState.FightStand + "/" + direction);
    }
    
    public void PlaySit(CreatureDirection direction)
    {
        Play(CreatureState.Sit + "/" + direction);
    }
    
    public void PlayStandUp(CreatureDirection direction)
    {
        PlayBackwards(CreatureState.Sit + "/" + direction);
    }
    
    public void PlayHurt(CreatureDirection direction)
    {
        Play(CreatureState.Hurt + "/" + direction);
    }
    
    public void PlayDie(CreatureDirection direction)
    {
        Play(CreatureState.Die + "/" + direction);
    }
    
    public void PlayHello(CreatureDirection direction)
    {
        Play(CreatureState.Hello + "/" + direction);
    }
    
    public void PlayKick(CreatureDirection direction)
    {
        Play(AttackAction.Kick + "/" + direction);
    }
    
    public void PlayPunch(CreatureDirection direction)
    {
        Play(AttackAction.Punch + "/" + direction);
    }
    
    public void PlaySword1HAttack(CreatureDirection direction)
    {
        Play(AttackAction.Sword1H + "/" + direction);
    }
    
    public void PlayBlade1HAttack(CreatureDirection direction)
    {
        Play(AttackAction.Sword1H + "/" + direction);
    }
    
    public void PlayBlade2HAttack(CreatureDirection direction)
    {
        Play(AttackAction.Blade2H + "/" + direction);
    }
    
    public void PlaySword2HAttack(CreatureDirection direction)
    {
        Play(AttackAction.Sword2H + "/" + direction);
    }
    
    public void PlayAexAttack(CreatureDirection direction)
    {
        Play(AttackAction.Axe + "/" + direction);
    }
    
    public void PlaySpearAttack(CreatureDirection direction)
    {
        Play(AttackAction.Axe + "/" + direction);
    }
    
    public void PlayBowAttack(CreatureDirection direction)
    {
        Play(AttackAction.Bow + "/" + direction);
    }
    
    public void PlayThrowAttack(CreatureDirection direction)
    {
        Play(AttackAction.Throw + "/" + direction);
    }
    
    // private void DisableNodeAnimation(int textureTack, int offsetTrack)
    // {
    //     string[] libraryNames = [MoveAction.Walk.ToString(), CreatureState.Idle.ToString(), MoveAction.FightWalk.ToString(),
    //         CreatureState.FightStand.ToString(), CreatureState.Sit.ToString(), CreatureState.Hurt.ToString(),
    //         CreatureState.Die.ToString(), CreatureState.Hello.ToString(), AttackAction.Kick.ToString(),
    //         AttackAction.Punch.ToString(), AttackAction.Sword1H.ToString(), AttackAction.Throw.ToString(),
    //         AttackAction.Blade2H.ToString(), AttackAction.Sword2H.ToString(),AttackAction.Axe.ToString(),
    //         AttackAction.Bow.ToString()
    //     ];
    //     Texture2D empty = new Texture2D();
    //     foreach (var libname in libraryNames)
    //     {
    //         var animationLibrary = GetAnimationLibrary(libname);
    //         foreach (var dir in Enum.GetValues(typeof(CreatureDirection)))
    //         {
    //             var animation = animationLibrary.GetAnimation(dir.ToString());
    //             int count = animation.TrackGetKeyCount(offsetTrack);
    //             for (int i = 0; i < count; i++)
    //             {
    //                 animation.TrackSetKeyValue(textureTack, i, empty);
    //                 animation.TrackSetKeyValue(offsetTrack, i, Vector2.Zero);
    //             }
    //         }
    //     }
    // }

    private void UpdateNodeAnimationLibrary(string libraryName, int textureTrack, int offsetTrack, Sprite[] sprites)
    {
        var animationLibrary = GetAnimationLibrary(libraryName);
        int index = 0;
        foreach (var dir in Enum.GetValues(typeof(CreatureDirection)))
        {
            var animation = animationLibrary.GetAnimation(dir.ToString());
            int count = animation.TrackGetKeyCount(offsetTrack);
            for (int i = 0; i < count; i++)
            {
                animation.TrackSetKeyValue(textureTrack, i, sprites[index].Texture);
                animation.TrackSetKeyValue(offsetTrack, i, sprites[index].Offset);
                index++;
            }
        }
    }


    private void UpdateNodeAnimation(string spriteName, int textureTrack, int offsetTrack)
    {
        var sprites = _spriteLoader.Load(spriteName + "0");
        UpdateNodeAnimationLibrary(MoveAction.Walk.ToString(), textureTrack, offsetTrack, sprites);
        
        var tmp = new Sprite[IdleSpriteNumber * DirectionNumber];
        int index = WalkSpriteNumber * DirectionNumber;
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(CreatureState.Idle.ToString(), textureTrack, offsetTrack, tmp);
        index += tmp.Length;
        
        tmp = new Sprite[FightWalkSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(MoveAction.FightWalk.ToString(), textureTrack, offsetTrack, tmp);
        index += tmp.Length;
        
        tmp = new Sprite[FightStandSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(CreatureState.FightStand.ToString(), textureTrack, offsetTrack, tmp);
        index += tmp.Length;
        
        tmp = new Sprite[SitSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(CreatureState.Sit.ToString(), textureTrack, offsetTrack, tmp);
        index += tmp.Length;
        
        tmp = new Sprite[HurtSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(CreatureState.Hurt.ToString(), textureTrack, offsetTrack, tmp);
        index += tmp.Length;
        
        tmp = new Sprite[DieSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index + DieSpriteNumber * (DirectionNumber - 1), tmp, 0, DieSpriteNumber);
        Array.Copy(sprites, index, tmp, DieSpriteNumber, tmp.Length - DieSpriteNumber);
        UpdateNodeAnimationLibrary(CreatureState.Die.ToString(), textureTrack, offsetTrack, tmp);
        index += tmp.Length;
        
        tmp = new Sprite[HelloSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(CreatureState.Hello.ToString(), textureTrack, offsetTrack, tmp);
        
        sprites = _spriteLoader.Load(spriteName + "1");
        UpdateNodeAnimationLibrary(AttackAction.Kick.ToString(), textureTrack, offsetTrack, sprites);
        index = KickSpriteNumber * DirectionNumber;
        
        tmp = new Sprite[PunchSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(AttackAction.Punch.ToString(), textureTrack, offsetTrack, tmp);
        
        sprites = _spriteLoader.Load(spriteName + "2");
        UpdateNodeAnimationLibrary(AttackAction.Sword1H.ToString(), textureTrack, offsetTrack, sprites);
        UpdateNodeAnimationLibrary(AttackAction.Throw.ToString(), textureTrack, offsetTrack, sprites);
        index = Sword1HSpriteNumber * DirectionNumber;
        
        tmp = new Sprite[Blade2HSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(AttackAction.Blade2H.ToString(), textureTrack, offsetTrack, tmp);
        index += tmp.Length;
        
        tmp = new Sprite[Sword2HSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(AttackAction.Sword2H.ToString(), textureTrack, offsetTrack, tmp);

        sprites = _spriteLoader.Load(spriteName + "3");
        UpdateNodeAnimationLibrary(AttackAction.Axe.ToString(), textureTrack, offsetTrack, sprites);
        
        sprites = _spriteLoader.Load(spriteName + "4");
        UpdateNodeAnimationLibrary(AttackAction.Bow.ToString(), textureTrack, offsetTrack, sprites);
    }

    public void SetHatAnimation(string name)
    {
        UpdateNodeAnimation(name, _hatTextureIdx, _hatOffsetIdx);
    }
    
    public void SetLegAnimation(string name)
    {
        UpdateNodeAnimation(name, _legTextureIdx, _legOffsetIdx);
    }
    public void SetBootAnimation(string name)
    {
        UpdateNodeAnimation(name, _bootTextureIdx, _bootOffsetIdx);
    }
    
    public void SetChestAnimation(string name)
    {
        UpdateNodeAnimation(name, _chestTextureIdx, _chestOffsetIdx);
    }
    
    public void SetArmorAnimation(string name)
    {
        UpdateNodeAnimation(name, _armorTextureIdx, _armorOffsetIdx);
    }

    public void SetWristAnimation(string l, string r)
    {
        UpdateNodeAnimation(l, _leftWristTextureIdx, _leftWristOffsetIdx);
        UpdateNodeAnimation(r, _rightWristTextureIdx, _rightWristOffsetIdx);
    }
}