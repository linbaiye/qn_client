using System;
using Godot;
using NLog;
using QnClient.code.entity;
using QnClient.code.sprite;
using QnClient.code.util;

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
    private int _vestTextureIdx = -1;
    private int _vestOffsetIdx = -1;
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
            _vestTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            _vestOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
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
            var mouseAreaPosition = animation.AddTrack(Animation.TrackType.Value);
            var mouseAreaSize = animation.AddTrack(Animation.TrackType.Value);
            animation.TrackSetPath(_bodyTextureIdx, "Body:texture");
            animation.TrackSetPath(_bodyOffsetIdx, "Body:offset");
            animation.TrackSetPath(mouseAreaPosition, "Body/MouseArea:position");
            animation.TrackSetPath(mouseAreaSize, "Body/MouseArea:size");
            animation.TrackSetPath(_legTextureIdx, "Leg:texture");
            animation.TrackSetPath(_legOffsetIdx, "Leg:offset");
            animation.TrackSetPath(_bootTextureIdx, "Boot:texture");
            animation.TrackSetPath(_bootOffsetIdx, "Boot:offset");
            animation.TrackSetPath(_leftWristTextureIdx, "LeftWrist:texture");
            animation.TrackSetPath(_leftWristOffsetIdx, "LeftWrist:offset");
            animation.TrackSetPath(_rightWristTextureIdx, "RightWrist:texture");
            animation.TrackSetPath(_rightWristOffsetIdx, "RightWrist:offset");
            animation.TrackSetPath(_vestTextureIdx, "Vest:texture");
            animation.TrackSetPath(_vestOffsetIdx, "Vest:offset");
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
            animation.ValueTrackSetUpdateMode(_vestTextureIdx, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(_vestOffsetIdx, Animation.UpdateMode.Discrete);
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
            animation.ValueTrackSetUpdateMode(mouseAreaPosition, Animation.UpdateMode.Discrete);
            animation.ValueTrackSetUpdateMode(mouseAreaSize, Animation.UpdateMode.Discrete);
            for (int i = 0; i < spritesPerDirection; i++)
            {
                var time = step * i;
                var textureOffset = sprites[start + i].Offset + VectorUtil.DefaultTextureOffset;
                animation.TrackInsertKey(_bodyTextureIdx, time, sprites[start + i].Texture);
                animation.TrackInsertKey(_bodyOffsetIdx, time, textureOffset);
                animation.TrackInsertKey(_legTextureIdx, time, empty);
                animation.TrackInsertKey(_legOffsetIdx, time, Vector2.Zero);
                animation.TrackInsertKey(_bootTextureIdx, time, empty);
                animation.TrackInsertKey(_bootOffsetIdx, time, Vector2.Zero);
                animation.TrackInsertKey(_leftWristTextureIdx, time, empty);
                animation.TrackInsertKey(_leftWristOffsetIdx, time, Vector2.Zero);
                animation.TrackInsertKey(_rightWristTextureIdx, time, empty);
                animation.TrackInsertKey(_rightWristOffsetIdx, time, Vector2.Zero);
                animation.TrackInsertKey(_vestTextureIdx, time, empty);
                animation.TrackInsertKey(_vestOffsetIdx, time, Vector2.Zero);
                animation.TrackInsertKey(_armorTextureIdx, time, empty);
                animation.TrackInsertKey(_armorOffsetIdx, time, Vector2.Zero);
                animation.TrackInsertKey(_hairTextureIdx, time, empty);
                animation.TrackInsertKey(_hairOffsetIdx, time, Vector2.Zero);
                animation.TrackInsertKey(_hatTextureIdx, time, empty);
                animation.TrackInsertKey(_hatOffsetIdx, time, Vector2.Zero);
                animation.TrackInsertKey(_weaponTextureIdx, time, empty);
                animation.TrackInsertKey(_weaponOffsetIdx, time, Vector2.Zero);
                animation.TrackInsertKey(_attackEffectTextureIdx, time, empty);
                animation.TrackInsertKey(_attackEffectOffsetIdx, time, Vector2.Zero);
                animation.TrackInsertKey(mouseAreaPosition, time, textureOffset);
                animation.TrackInsertKey(mouseAreaSize, time, sprites[start + i].OriginalSize);
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
        AddAnimationLibrary(PlayerState.Idle.ToString(), CreateAnimationLibrary(IdleSpriteNumber, IdleStep, tmp, Animation.LoopModeEnum.Linear));
        index += tmp.Length;
        
        tmp = new Sprite[FightWalkSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(MoveAction.FightWalk.ToString(), CreateAnimationLibrary(FightWalkSpriteNumber, FightWalkStep, tmp));
        index += tmp.Length;
        
        tmp = new Sprite[FightStandSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(PlayerState.FightStand.ToString(), CreateAnimationLibrary(FightStandSpriteNumber, FightStandStep, tmp, Animation.LoopModeEnum.Linear));
        index += tmp.Length;
        
        tmp = new Sprite[SitSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(PlayerState.Sit.ToString(), CreateAnimationLibrary(SitSpriteNumber , SitStep, tmp));
        index += tmp.Length;
        
        tmp = new Sprite[HurtSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(PlayerState.Hurt.ToString(), CreateAnimationLibrary(HurtSpriteNumber, HurtStep, tmp));
        index += tmp.Length;
        
        tmp = new Sprite[DieSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index + DieSpriteNumber * (DirectionNumber - 1), tmp, 0, DieSpriteNumber);
        Array.Copy(sprites, index, tmp, DieSpriteNumber, tmp.Length - DieSpriteNumber);
        AddAnimationLibrary(PlayerState.Die.ToString(), CreateAnimationLibrary(DieSpriteNumber, DieStep, tmp));
        index += tmp.Length;
        
        tmp = new Sprite[HelloSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(PlayerState.Hello.ToString(), CreateAnimationLibrary(HelloSpriteNumber, HelloStep, tmp));
        
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


    
    public void PlayWalkFrom(CreatureDirection direction, int fromMillis)
    {
        PlayAnimation(MoveAction.Walk + "/" + direction, fromMillis);
    }

    public void PlayRun(CreatureDirection direction)
    {
        Play(MoveAction.Walk + "/" + direction, -1, 2f);
    }
    
    public void PlayRunFrom(CreatureDirection direction, int fromMillis)
    {
        PlayAnimation(MoveAction.Walk + "/" + direction, fromMillis, 2f);
    }

    public void PlayFly(CreatureDirection direction)
    {
        Play(PlayerState.Idle + "/" + direction, -1, 2f);
    }
    
    public void PlayFlyFrom(CreatureDirection direction, int fromMillis)
    {
        PlayAnimation(PlayerState.Idle + "/" + direction, fromMillis, 2f);
    }
    
    public void PlayIdle(CreatureDirection direction)
    {
        Play(PlayerState.Idle + "/" + direction);
    }
    
    
    public void PlayIdleFrom(CreatureDirection direction, int fromMillis)
    {
        PlayAnimation(PlayerState.Idle + "/" + direction, fromMillis);
    }


    public void PlayFightWalk(CreatureDirection direction)
    {
        Play(MoveAction.FightWalk + "/" + direction);
    }
    
    public void PlayFightWalkFrom(CreatureDirection direction, int fromMillis)
    {
        PlayAnimation(MoveAction.FightWalk + "/" + direction, fromMillis);
    }
    
    public void PlayFightStand(CreatureDirection direction)
    {
        Play(PlayerState.FightStand + "/" + direction);
    }
    
    public void PlayFightStandFrom(CreatureDirection direction, int fromMillis)
    {
        PlayAnimation(PlayerState.FightStand + "/" + direction, fromMillis);
    }
    
    public void PlaySit(CreatureDirection direction)
    {
        Play(PlayerState.Sit + "/" + direction);
    }
    
    public void PlaySitFrom(CreatureDirection direction, int fromMillis)
    {
        PlayAnimation(PlayerState.Sit + "/" + direction, fromMillis);
    }
    
    public void PlayStandUp(CreatureDirection direction)
    {
        PlayBackwards(PlayerState.Sit + "/" + direction);
    }
    
    public void PlayStandUpFrom(CreatureDirection direction, int fromMillis)
    {
        var name = PlayerState.Sit + "/" + direction;
        var animation = GetAnimation(name);
        int aniLengthMillis = (int)(animation.Length * 1000);
        int startMillis = fromMillis % aniLengthMillis;
        PlaySectionBackwards(name, startMillis);
    }
    
    public void PlayHurt(CreatureDirection direction)
    {
        Play(PlayerState.Hurt + "/" + direction);
    }

    public void PlayHurtFrom(CreatureDirection direction, int fromMillis)
    {
        PlayAnimation(PlayerState.Hurt + "/" + direction, fromMillis);
    }

    public void PlayDie(CreatureDirection direction)
    {
        Play(PlayerState.Die + "/" + direction);
    }
    
    public void PlayDieFrom(CreatureDirection direction, int fromMillis)
    {
        PlayAnimation(PlayerState.Die + "/" + direction, fromMillis);
    }
    
    public void PlayHello(CreatureDirection direction)
    {
        Play(PlayerState.Hello + "/" + direction);
    }
    
    public void PlayHelloFrom(CreatureDirection direction, int fromMillis)
    {
        PlayAnimation(PlayerState.Hello + "/" + direction, fromMillis);
    }

    
    public void PlayKick(CreatureDirection direction)
    {
        Play(AttackAction.Kick + "/" + direction);
    }
    
    public void PlayKickFrom(CreatureDirection direction, int fromMillis)
    {
        PlayAnimation(AttackAction.Kick + "/" + direction, fromMillis);
    }
    
    public void PlayPunch(CreatureDirection direction)
    {
        Play(AttackAction.Punch + "/" + direction);
    }
    
    public void PlayPunchFrom(CreatureDirection direction, int fromMillis)
    {
        PlayAnimation(AttackAction.Punch + "/" + direction, fromMillis);
    }
    
    public void PlaySword1HAttack(CreatureDirection direction)
    {
        Play(AttackAction.Sword1H + "/" + direction);
    }
    
    public void PlaySword1HAttackFrom(CreatureDirection direction, int fromMillis)
    {
        PlayAnimation(AttackAction.Sword1H + "/" + direction, fromMillis);
    }
    
    public void PlayBlade1HAttack(CreatureDirection direction)
    {
        Play(AttackAction.Sword1H + "/" + direction);
    }
    
    public void PlayBlade1HAttackFrom(CreatureDirection direction, int fromMillis)
    {
        PlayAnimation(AttackAction.Sword1H + "/" + direction, fromMillis);
    }
    
    public void PlayBlade2HAttack(CreatureDirection direction)
    {
        Play(AttackAction.Blade2H + "/" + direction);
    }
    
    public void PlayBlade2HAttackFrom(CreatureDirection direction, int fromMillis)
    {
        PlayAnimation(AttackAction.Blade2H + "/" + direction, fromMillis);
    }

    
    public void PlaySword2HAttack(CreatureDirection direction)
    {
        Play(AttackAction.Sword2H + "/" + direction);
    }
    
    public void PlaySword2HAttackFrom(CreatureDirection direction, int fromMillis)
    {
        PlayAnimation(AttackAction.Sword2H + "/" + direction, fromMillis);
    }
    
    public void PlayAxeAttack(CreatureDirection direction)
    {
        Play(AttackAction.Axe + "/" + direction);
    }
    
    public void PlayAxeAttackFrom(CreatureDirection direction, int fromMillis)
    {
        PlayAnimation(AttackAction.Axe + "/" + direction, fromMillis);
    }
    
    public void PlaySpearAttack(CreatureDirection direction)
    {
        Play(AttackAction.Axe + "/" + direction);
    }
    
    public void PlaySpearAttackFrom(CreatureDirection direction, int fromMillis)
    {
        PlayAnimation(AttackAction.Axe + "/" + direction, fromMillis);
    }
    
    public void PlayBowAttack(CreatureDirection direction)
    {
        Play(AttackAction.Bow + "/" + direction);
    }
    
    public void PlayBowAttackFrom(CreatureDirection direction, int fromMillis)
    {
        PlayAnimation(AttackAction.Bow + "/" + direction, fromMillis);
    }
    
    
    public void PlayThrowAttack(CreatureDirection direction)
    {
        Play(AttackAction.Throw + "/" + direction);
    }

    public void PlayThrowAttackFrom(CreatureDirection direction, int fromMillis)
    {
        PlayAnimation(AttackAction.Throw + "/" + direction, fromMillis);
    }

    private void PlayAnimation(string name, int millis, float speed = 1)
    {
        var animation = GetAnimation(name);
        int aniLengthMillis = (int)(animation.Length * 1000);
        int startMillis = millis % aniLengthMillis;
        PlaySection(name, startMillis, -1, -1, speed);
    }
    

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
                animation.TrackSetKeyValue(offsetTrack, i, sprites[index].Offset + VectorUtil.DefaultTextureOffset);
                index++;
            }
        }
    }

    private void UpdateNodeCommonAnimation(string spriteName, int textureTrack, int offsetTrack, bool hasHello = true)
    {
        var sprites = _spriteLoader.Load(spriteName);
        UpdateNodeAnimationLibrary(MoveAction.Walk.ToString(), textureTrack, offsetTrack, sprites);
        
        var tmp = new Sprite[IdleSpriteNumber * DirectionNumber];
        int index = WalkSpriteNumber * DirectionNumber;
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(PlayerState.Idle.ToString(), textureTrack, offsetTrack, tmp);
        index += tmp.Length;
        
        tmp = new Sprite[FightWalkSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(MoveAction.FightWalk.ToString(), textureTrack, offsetTrack, tmp);
        index += tmp.Length;
        
        tmp = new Sprite[FightStandSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(PlayerState.FightStand.ToString(), textureTrack, offsetTrack, tmp);
        index += tmp.Length;
        
        tmp = new Sprite[SitSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(PlayerState.Sit.ToString(), textureTrack, offsetTrack, tmp);
        index += tmp.Length;
        
        tmp = new Sprite[HurtSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(PlayerState.Hurt.ToString(), textureTrack, offsetTrack, tmp);
        index += tmp.Length;
        
        tmp = new Sprite[DieSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index + DieSpriteNumber * (DirectionNumber - 1), tmp, 0, DieSpriteNumber);
        Array.Copy(sprites, index, tmp, DieSpriteNumber, tmp.Length - DieSpriteNumber);
        UpdateNodeAnimationLibrary(PlayerState.Die.ToString(), textureTrack, offsetTrack, tmp);
        index += tmp.Length;

        if (hasHello)
        {
            tmp = new Sprite[HelloSpriteNumber * DirectionNumber];
            Array.Copy(sprites, index, tmp, 0, tmp.Length);
            UpdateNodeAnimationLibrary(PlayerState.Hello.ToString(), textureTrack, offsetTrack, tmp);
        }
    }

    private void UpdateNodeAnimation(string spritePrefix, int textureTrack, int offsetTrack)
    {
        UpdateNodeCommonAnimation(spritePrefix + "0", textureTrack, offsetTrack);
        var sprites = _spriteLoader.Load(spritePrefix + "1");
        UpdateNodeAnimationLibrary(AttackAction.Kick.ToString(), textureTrack, offsetTrack, sprites);
        int index = KickSpriteNumber * DirectionNumber;
        
        var tmp = new Sprite[PunchSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(AttackAction.Punch.ToString(), textureTrack, offsetTrack, tmp);
        
        sprites = _spriteLoader.Load(spritePrefix + "2");
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

        sprites = _spriteLoader.Load(spritePrefix + "3");
        UpdateNodeAnimationLibrary(AttackAction.Axe.ToString(), textureTrack, offsetTrack, sprites);
        
        sprites = _spriteLoader.Load(spritePrefix + "4");
        UpdateNodeAnimationLibrary(AttackAction.Bow.ToString(), textureTrack, offsetTrack, sprites);
    }

    private void UpdateWeaponNodeAttackAnimation(string spriteName, string attackAction, int textureTrack, int offsetTrack)
    {
        var sprites = _spriteLoader.Load(spriteName);
        UpdateNodeAnimationLibrary(attackAction, textureTrack, offsetTrack, sprites);
    }


    public void SetBladeAnimation(string spritePrefix)
    {
        UpdateNodeCommonAnimation(spritePrefix + "0", _weaponTextureIdx, _weaponOffsetIdx, false);
        var sprites = _spriteLoader.Load(spritePrefix + "2");
        UpdateNodeAnimationLibrary(AttackAction.Sword1H.ToString(), _weaponTextureIdx, _weaponOffsetIdx, sprites);
        var tmp = new Sprite[Blade2HSpriteNumber * DirectionNumber];
        Array.Copy(sprites, Sword1HSpriteNumber * DirectionNumber, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(AttackAction.Blade2H.ToString(), _weaponTextureIdx, _weaponOffsetIdx, tmp);
    }
    
    public void SetSwordAnimation(string spritePrefix)
    {
        UpdateNodeCommonAnimation(spritePrefix + "0", _weaponTextureIdx, _weaponOffsetIdx, false);
        var sprites = _spriteLoader.Load(spritePrefix + "2");
        UpdateNodeAnimationLibrary(AttackAction.Sword1H.ToString(), _weaponTextureIdx, _weaponOffsetIdx, sprites);
        var tmp = new Sprite[Sword2HSpriteNumber * DirectionNumber];
        Array.Copy(sprites, (Sword1HSpriteNumber + Blade2HSpriteNumber) * DirectionNumber, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(AttackAction.Sword2H.ToString(), _weaponTextureIdx, _weaponOffsetIdx, tmp);
    }
    
    private void SetAxeOrSpearAnimation(string spritePrefix)
    {
        UpdateNodeCommonAnimation(spritePrefix + "0", _weaponTextureIdx, _weaponOffsetIdx, false);
        var sprites = _spriteLoader.Load(spritePrefix + "3");
        UpdateNodeAnimationLibrary(AttackAction.Axe.ToString(), _weaponTextureIdx, _weaponOffsetIdx, sprites);
    }
    
    public void SetSpearAnimation(string spritePrefix)
    {
        SetAxeOrSpearAnimation(spritePrefix);
    }
    
    public void SetAxeAnimation(string spritePrefix)
    {
        SetAxeOrSpearAnimation(spritePrefix);
    }

    public void SetBowAnimation(string spritePrefix)
    {
        UpdateNodeCommonAnimation(spritePrefix + "0", _weaponTextureIdx, _weaponOffsetIdx, false);
        var sprites = _spriteLoader.Load(spritePrefix + "4");
        UpdateNodeAnimationLibrary(AttackAction.Bow.ToString(), _weaponTextureIdx, _weaponOffsetIdx, sprites);
    }
    
    public void SetThrowAnimation(string spritePrefix)
    {
        UpdateNodeCommonAnimation(spritePrefix + "0", _weaponTextureIdx, _weaponOffsetIdx, false);
        var sprites = _spriteLoader.Load(spritePrefix + "2");
        UpdateNodeAnimationLibrary(AttackAction.Throw.ToString(), _weaponTextureIdx, _weaponOffsetIdx, sprites);
    }

    public void SetHatAnimation(string prefix)
    {
        UpdateNodeAnimation(prefix, _hatTextureIdx, _hatOffsetIdx);
    }
    
    public void SetLegAnimation(string prefix)
    {
        UpdateNodeAnimation(prefix, _legTextureIdx, _legOffsetIdx);
    }
    
    public void SetBootAnimation(string prefix)
    {
        UpdateNodeAnimation(prefix, _bootTextureIdx, _bootOffsetIdx);
    }
    
    public void SetVestAnimation(string prefix)
    {
        UpdateNodeAnimation(prefix, _vestTextureIdx, _vestOffsetIdx);
    }
    
    public void SetArmorAnimation(string prefix)
    {
        UpdateNodeAnimation(prefix, _armorTextureIdx, _armorOffsetIdx);
    }
    
    public void SetHairAnimation(string prefix)
    {
        UpdateNodeAnimation(prefix, _hairTextureIdx, _hairOffsetIdx);
    }

    public void SetFistWeaponAnimation(string prefix)
    {
        UpdateNodeCommonAnimation(prefix + "0", _weaponTextureIdx, _weaponOffsetIdx, false);
        var sprites = _spriteLoader.Load(prefix + "1");
        UpdateNodeAnimationLibrary(AttackAction.Kick.ToString(), _weaponTextureIdx, _weaponOffsetIdx, sprites);
        int index = KickSpriteNumber * DirectionNumber;
        var tmp = new Sprite[PunchSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(AttackAction.Punch.ToString(), _weaponTextureIdx, _weaponOffsetIdx, tmp);
    }

    public void SetWristAnimation(string l, string r)
    {
        UpdateNodeAnimation(l, _leftWristTextureIdx, _leftWristOffsetIdx);
        UpdateNodeAnimation(r, _rightWristTextureIdx, _rightWristOffsetIdx);
    }

    public void SetBladeEffectAnimation(string name)
    {
        var sprites = _spriteLoader.Load(name);
        UpdateNodeAnimationLibrary(AttackAction.Sword1H.ToString(), _attackEffectTextureIdx, _attackEffectOffsetIdx, sprites);
        int index = Sword1HSpriteNumber * DirectionNumber;
        var tmp = new Sprite[Sword1HSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(AttackAction.Blade2H.ToString(), _attackEffectTextureIdx, _attackEffectOffsetIdx, tmp);
    }
}