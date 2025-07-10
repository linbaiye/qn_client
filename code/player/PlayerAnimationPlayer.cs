using System;
using Godot;
using NLog;
using QnClient.code.entity;
using QnClient.code.sprite;
using QnClient.code.util;
using Array = System.Array;

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

    private StringName _finished;

    private string _effect;

    public override void _Ready()
    {
        _spriteLoader = ZipFileSpriteLoader.Instance;
        AnimationFinished += n => _finished = n;
    }
    

    // private void AddCallbackTrackAtTime(AnimationLibrary animationLibrary, float time)
    // {
    //     foreach (var dir in Enum.GetValues(typeof(CreatureDirection)))
    //     {
    //         var animation = animationLibrary.GetAnimation(dir.ToString());
    //         var callbackTack = animation.AddTrack(Animation.TrackType.Method);
    //         animation.TrackSetPath(callbackTack, "AnimationPlayer");
    //         var methodDictionary = new Godot.Collections.Dictionary
    //         {
    //             { "method", MethodName.Callback },
    //             { "args", new Godot.Collections.Array() {} }
    //         };
    //         animation.TrackInsertKey(callbackTack, time, methodDictionary, 0);
    //     }
    // }


    private int AddTrack(Animation animation, string path)
    {
        var trackId = animation.AddTrack(Animation.TrackType.Value);
        animation.TrackSetPath(trackId, path);
        animation.ValueTrackSetUpdateMode(trackId, Animation.UpdateMode.Capture);
        animation.TrackSetInterpolationType(trackId, Animation.InterpolationType.Nearest);
        return trackId;
    }

    private AnimationLibrary CreateAnimationLibrary(int spritesPerDirection,
        float step,
        OffsetTexture[] sprites,
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
            _bodyTextureIdx = AddTrack(animation, "Body:texture");
            _bodyOffsetIdx = AddTrack(animation, "Body:offset");
            _legTextureIdx = AddTrack(animation, "Leg:texture");
            _legOffsetIdx = AddTrack(animation, "Leg:offset");
            _bootTextureIdx = AddTrack(animation, "Boot:texture");
            _bootOffsetIdx = AddTrack(animation, "Boot:offset");
            _leftWristTextureIdx = AddTrack(animation, "LeftWrist:texture");
            _leftWristOffsetIdx = AddTrack(animation, "LeftWrist:offset");
            _rightWristTextureIdx = AddTrack(animation, "RightWrist:texture");
            _rightWristOffsetIdx = AddTrack(animation, "RightWrist:offset");
            _vestTextureIdx = AddTrack(animation, "Vest:texture");
            _vestOffsetIdx = AddTrack(animation, "Vest:offset");
            _armorTextureIdx = AddTrack(animation, "Armor:texture");
            _armorOffsetIdx = AddTrack(animation, "Armor:offset");
            _hairTextureIdx = AddTrack(animation, "Hair:texture");
            _hairOffsetIdx = AddTrack(animation, "Hair:offset");
            _hatTextureIdx = AddTrack(animation, "Hat:texture");
            _hatOffsetIdx = AddTrack(animation, "Hat:offset");
            _weaponTextureIdx = AddTrack(animation, "Weapon:texture");
            _weaponOffsetIdx = AddTrack(animation, "Weapon:offset");
            _attackEffectTextureIdx = animation.AddTrack(Animation.TrackType.Value);
            _attackEffectOffsetIdx = animation.AddTrack(Animation.TrackType.Value);
            var mouseAreaPosition = animation.AddTrack(Animation.TrackType.Value);
            var mouseAreaSize = animation.AddTrack(Animation.TrackType.Value);
            animation.TrackSetPath(mouseAreaPosition, "Body/MouseArea:position");
            animation.TrackSetPath(mouseAreaSize, "Body/MouseArea:size");
            animation.TrackSetPath(_attackEffectTextureIdx, "AttackEffect:texture");
            animation.TrackSetPath(_attackEffectOffsetIdx, "AttackEffect:offset");
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

    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();


    private OffsetTexture[] ExtractStandUpOffsetTextures(OffsetTexture[] sitTextures)
    {
        OffsetTexture[] standUpTextures = new OffsetTexture[sitTextures.Length];
        for (int i = 0, k = 0; i < DirectionNumber; i++)
        {
            int index = SitSpriteNumber * (i + 1);
            for (int j = 1; j <= SitSpriteNumber; j++)
            {
                standUpTextures[k++] = sitTextures[index - j];
            }
        }

        return standUpTextures;
    }

    
    public void InitializeAnimations(bool male)
    {
        string prefix = male ? "N0" : "A0";
        var sprites = _spriteLoader.Load(prefix + "0");
        var animationLibrary = CreateAnimationLibrary(WalkSpriteNumber, WalkStep, sprites);
        AddAnimationLibrary(MoveAction.Walk.ToString(), animationLibrary);

        var tmp = new OffsetTexture[IdleSpriteNumber * DirectionNumber];
        int index = WalkSpriteNumber * DirectionNumber;
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(PlayerState.Idle.ToString(), CreateAnimationLibrary(IdleSpriteNumber, IdleStep, tmp, Animation.LoopModeEnum.Linear));
        index += tmp.Length;
        
        tmp = new OffsetTexture[FightWalkSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        animationLibrary = CreateAnimationLibrary(FightWalkSpriteNumber, FightWalkStep, tmp);
        AddAnimationLibrary(MoveAction.FightWalk.ToString(), animationLibrary);
        index += tmp.Length;
        
        tmp = new OffsetTexture[FightStandSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(PlayerState.FightStand.ToString(), CreateAnimationLibrary(FightStandSpriteNumber, FightStandStep, tmp, Animation.LoopModeEnum.Linear));
        index += tmp.Length;
        
        tmp = new OffsetTexture[SitSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        var library = CreateAnimationLibrary(SitSpriteNumber , SitStep, tmp);
        AddAnimationLibrary(PlayerState.Sit.ToString(), library);
        AddAnimationLibrary(PlayerState.StandUp.ToString(), CreateAnimationLibrary(SitSpriteNumber, SitStep, ExtractStandUpOffsetTextures(tmp)));
        index += tmp.Length;
        
        tmp = new OffsetTexture[HurtSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(PlayerState.Hurt.ToString(), CreateAnimationLibrary(HurtSpriteNumber, HurtStep, tmp));
        index += tmp.Length;
        
        tmp = new OffsetTexture[DieSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index + DieSpriteNumber * (DirectionNumber - 1), tmp, 0, DieSpriteNumber);
        Array.Copy(sprites, index, tmp, DieSpriteNumber, tmp.Length - DieSpriteNumber);
        AddAnimationLibrary(PlayerState.Die.ToString(), CreateAnimationLibrary(DieSpriteNumber, DieStep, tmp));
        index += tmp.Length;
        
        tmp = new OffsetTexture[HelloSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(PlayerState.Hello.ToString(), CreateAnimationLibrary(HelloSpriteNumber, HelloStep, tmp));
        
        sprites = _spriteLoader.Load(prefix + "1");
        AddAnimationLibrary(AttackAction.Kick.ToString(), CreateAnimationLibrary(KickSpriteNumber, KickStep, sprites));
        index = KickSpriteNumber * DirectionNumber;
        
        tmp = new OffsetTexture[PunchSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(AttackAction.Punch.ToString(), CreateAnimationLibrary(PunchSpriteNumber, PunchStep, tmp));
        
        sprites = _spriteLoader.Load(prefix + "2");
        AddAnimationLibrary(AttackAction.Sword1H.ToString(), CreateAnimationLibrary(Sword1HSpriteNumber, Sword1HStep, sprites));
        AddAnimationLibrary(AttackAction.Throw.ToString(), CreateAnimationLibrary(ThrowSpriteNumber, ThrowStep, sprites));
        index = Sword1HSpriteNumber * DirectionNumber;
        
        tmp = new OffsetTexture[Blade2HSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(AttackAction.Blade2H.ToString(), CreateAnimationLibrary(Blade2HSpriteNumber, Blade2HStep, tmp));
        index += tmp.Length;
        
        tmp = new OffsetTexture[Sword2HSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        AddAnimationLibrary(AttackAction.Sword2H.ToString(), CreateAnimationLibrary(Sword2HSpriteNumber, Sword2HStep, tmp));

        sprites = _spriteLoader.Load(prefix + "3");
        AddAnimationLibrary(AttackAction.Axe.ToString(), CreateAnimationLibrary(AxeSpriteNumber, AxeStep, sprites));
        
        sprites = _spriteLoader.Load(prefix + "4");
        AddAnimationLibrary(AttackAction.Bow.ToString(), CreateAnimationLibrary(BowSpriteNumber, BowStep, sprites));
    }

    
    public void PlayWalk(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(MoveAction.Walk + "/" + direction, fromMillis);
    }
    
    public void PlayRun(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(MoveAction.Walk + "/" + direction, fromMillis, 2f);
    }

    
    public void PlayFly(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(PlayerState.Idle + "/" + direction, fromMillis, 2f);
    }
    
    
    
    public void PlayIdle(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(PlayerState.Idle + "/" + direction, fromMillis);
    }


    public void PlayFightWalk(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(MoveAction.FightWalk + "/" + direction, fromMillis);
    }
    
    public void PlayFightStand(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(PlayerState.FightStand + "/" + direction, fromMillis);
    }
    
    public void PlaySit(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(PlayerState.Sit + "/" + direction, fromMillis);
    }
    
    public void PlayStandUp(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(PlayerState.StandUp + "/" + direction, fromMillis);
    }
    

    public void PlayHurt(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(PlayerState.Hurt + "/" + direction, fromMillis);
    }
    
    public void PlayDie(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(PlayerState.Die + "/" + direction, fromMillis);
    }
    
    
    public void PlayHello(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(PlayerState.Hello + "/" + direction, fromMillis);
    }

    
    public void PlayKick(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(AttackAction.Kick + "/" + direction, fromMillis);
    }
    
    public void PlayPunch(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(AttackAction.Punch + "/" + direction, fromMillis);
    }
    
    
    public void PlaySword1HAttack(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(AttackAction.Sword1H + "/" + direction, fromMillis);
    }
    
    
    public void PlayBlade1HAttack(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(AttackAction.Sword1H + "/" + direction, fromMillis);
    }
    
    
    public void PlayBlade2HAttack(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(AttackAction.Blade2H + "/" + direction, fromMillis);
    }
    
    public void PlaySword2HAttack(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(AttackAction.Sword2H + "/" + direction, fromMillis);
    }
    
    
    public void PlayAxeAttack(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(AttackAction.Axe + "/" + direction, fromMillis);
    }
    
    
    public void PlaySpearAttack(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(AttackAction.Axe + "/" + direction, fromMillis);
    }
    
    
    public void PlayBowAttack(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(AttackAction.Bow + "/" + direction, fromMillis);
    }
    

    public void PlayThrowAttack(CreatureDirection direction, int fromMillis = 0)
    {
        PlayAnimation(AttackAction.Throw + "/" + direction, fromMillis);
    }

    private void PlayAnimation(string name, int millis, float speed = 1)
    {
        _finished = "";
        var animation = GetAnimation(name);
        int aniLengthMillis = (int)(animation.Length * 1000);
        int startMillis = millis % aniLengthMillis;
        if (!string.IsNullOrEmpty(CurrentAnimation))
            Stop(true);
        //PlayWithCapture(name, -1, -1, 1, false, Tween.TransitionType.Cubic);
        PlaySection(name, startMillis, -1, -1, speed);
    }
    

    private void UpdateNodeAnimationLibrary(string libraryName, int textureTrack, int offsetTrack, OffsetTexture[] sprites)
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
        
        var tmp = new OffsetTexture[IdleSpriteNumber * DirectionNumber];
        int index = WalkSpriteNumber * DirectionNumber;
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(PlayerState.Idle.ToString(), textureTrack, offsetTrack, tmp);
        index += tmp.Length;
        
        tmp = new OffsetTexture[FightWalkSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(MoveAction.FightWalk.ToString(), textureTrack, offsetTrack, tmp);
        index += tmp.Length;
        
        tmp = new OffsetTexture[FightStandSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(PlayerState.FightStand.ToString(), textureTrack, offsetTrack, tmp);
        index += tmp.Length;
        
        tmp = new OffsetTexture[SitSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(PlayerState.Sit.ToString(), textureTrack, offsetTrack, tmp);
        UpdateNodeAnimationLibrary(PlayerState.StandUp.ToString(), textureTrack, offsetTrack, ExtractStandUpOffsetTextures(tmp));
        index += tmp.Length;
        
        tmp = new OffsetTexture[HurtSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(PlayerState.Hurt.ToString(), textureTrack, offsetTrack, tmp);
        index += tmp.Length;
        
        tmp = new OffsetTexture[DieSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index + DieSpriteNumber * (DirectionNumber - 1), tmp, 0, DieSpriteNumber);
        Array.Copy(sprites, index, tmp, DieSpriteNumber, tmp.Length - DieSpriteNumber);
        UpdateNodeAnimationLibrary(PlayerState.Die.ToString(), textureTrack, offsetTrack, tmp);
        index += tmp.Length;

        if (hasHello)
        {
            tmp = new OffsetTexture[HelloSpriteNumber * DirectionNumber];
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
        
        var tmp = new OffsetTexture[PunchSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(AttackAction.Punch.ToString(), textureTrack, offsetTrack, tmp);
        
        sprites = _spriteLoader.Load(spritePrefix + "2");
        UpdateNodeAnimationLibrary(AttackAction.Sword1H.ToString(), textureTrack, offsetTrack, sprites);
        UpdateNodeAnimationLibrary(AttackAction.Throw.ToString(), textureTrack, offsetTrack, sprites);
        index = Sword1HSpriteNumber * DirectionNumber;
        
        tmp = new OffsetTexture[Blade2HSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(AttackAction.Blade2H.ToString(), textureTrack, offsetTrack, tmp);
        index += tmp.Length;
        
        tmp = new OffsetTexture[Sword2HSpriteNumber * DirectionNumber];
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
        var tmp = new OffsetTexture[Blade2HSpriteNumber * DirectionNumber];
        Array.Copy(sprites, Sword1HSpriteNumber * DirectionNumber, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(AttackAction.Blade2H.ToString(), _weaponTextureIdx, _weaponOffsetIdx, tmp);
        UpdateWeaponSpriteIfSitFinished();
    }
    
    public void SetSwordAnimation(string spritePrefix)
    {
        UpdateNodeCommonAnimation(spritePrefix + "0", _weaponTextureIdx, _weaponOffsetIdx, false);
        var sprites = _spriteLoader.Load(spritePrefix + "2");
        UpdateNodeAnimationLibrary(AttackAction.Sword1H.ToString(), _weaponTextureIdx, _weaponOffsetIdx, sprites);
        var tmp = new OffsetTexture[Sword2HSpriteNumber * DirectionNumber];
        Array.Copy(sprites, (Sword1HSpriteNumber + Blade2HSpriteNumber) * DirectionNumber, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(AttackAction.Sword2H.ToString(), _weaponTextureIdx, _weaponOffsetIdx, tmp);
        UpdateWeaponSpriteIfSitFinished();
    }
    
    private void SetAxeOrSpearAnimation(string spritePrefix)
    {
        UpdateNodeCommonAnimation(spritePrefix + "0", _weaponTextureIdx, _weaponOffsetIdx, false);
        var sprites = _spriteLoader.Load(spritePrefix + "3");
        UpdateNodeAnimationLibrary(AttackAction.Axe.ToString(), _weaponTextureIdx, _weaponOffsetIdx, sprites);
        UpdateWeaponSpriteIfSitFinished();
    }

    private void UpdateSpriteIfSitFinished(string nodeName, int textureTrack, int offsetTrack)
    {
        if (_finished.ToString().StartsWith(PlayerState.Sit.ToString()))
        {
            var animation = GetAnimation(_finished);
            var count = animation.TrackGetKeyCount(textureTrack);
            var texture = (Texture2D)animation.TrackGetKeyValue(textureTrack, count - 1);
            var offset = (Vector2)animation.TrackGetKeyValue(offsetTrack, count - 1);
            var sprite2D = GetNode<Sprite2D>("../" + nodeName);
            sprite2D.Texture = texture;
            sprite2D.Offset = offset;   
        }
    }

    private void UpdateWeaponSpriteIfSitFinished()
    {
        UpdateSpriteIfSitFinished("Weapon", _weaponTextureIdx, _weaponOffsetIdx);
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
        UpdateWeaponSpriteIfSitFinished();
    }
    
    public void SetThrowAnimation(string spritePrefix)
    {
        UpdateNodeCommonAnimation(spritePrefix + "0", _weaponTextureIdx, _weaponOffsetIdx, false);
        var sprites = _spriteLoader.Load(spritePrefix + "2");
        UpdateNodeAnimationLibrary(AttackAction.Throw.ToString(), _weaponTextureIdx, _weaponOffsetIdx, sprites);
        UpdateWeaponSpriteIfSitFinished();
    }

    public void SetHatAnimation(string prefix)
    {
        UpdateNodeAnimation(prefix, _hatTextureIdx, _hatOffsetIdx);
        UpdateSpriteIfSitFinished("Hat", _hatTextureIdx, _hatOffsetIdx);
    }
    
    public void SetLegAnimation(string prefix)
    {
        UpdateNodeAnimation(prefix, _legTextureIdx, _legOffsetIdx);
        UpdateSpriteIfSitFinished("Leg", _legTextureIdx, _legOffsetIdx);
    }
    
    public void SetBootAnimation(string prefix)
    {
        UpdateNodeAnimation(prefix, _bootTextureIdx, _bootOffsetIdx);
        UpdateSpriteIfSitFinished("Boot", _bootTextureIdx, _bootOffsetIdx);
    }
    
    public void SetVestAnimation(string prefix)
    {
        UpdateNodeAnimation(prefix, _vestTextureIdx, _vestOffsetIdx);
        UpdateSpriteIfSitFinished("Vest", _vestTextureIdx, _vestOffsetIdx);
    }
    
    public void SetArmorAnimation(string prefix)
    {
        UpdateNodeAnimation(prefix, _armorTextureIdx, _armorOffsetIdx);
        UpdateSpriteIfSitFinished("Armor", _armorTextureIdx, _armorOffsetIdx);
    }
    
    public void SetHairAnimation(string prefix)
    {
        UpdateNodeAnimation(prefix, _hairTextureIdx, _hairOffsetIdx);
        UpdateSpriteIfSitFinished("Hair", _hairTextureIdx, _hairOffsetIdx);
    }

    public void SetFistWeaponAnimation(string prefix)
    {
        UpdateNodeCommonAnimation(prefix + "0", _weaponTextureIdx, _weaponOffsetIdx, false);
        var sprites = _spriteLoader.Load(prefix + "1");
        UpdateNodeAnimationLibrary(AttackAction.Kick.ToString(), _weaponTextureIdx, _weaponOffsetIdx, sprites);
        int index = KickSpriteNumber * DirectionNumber;
        var tmp = new OffsetTexture[PunchSpriteNumber * DirectionNumber];
        Array.Copy(sprites, index, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(AttackAction.Punch.ToString(), _weaponTextureIdx, _weaponOffsetIdx, tmp);
        UpdateWeaponSpriteIfSitFinished();
    }

    public void SetWristAnimation(string l, string r)
    {
        UpdateNodeAnimation(l, _leftWristTextureIdx, _leftWristOffsetIdx);
        UpdateNodeAnimation(r, _rightWristTextureIdx, _rightWristOffsetIdx);
        UpdateSpriteIfSitFinished("LeftWrist", _leftWristTextureIdx, _leftWristOffsetIdx);
        UpdateSpriteIfSitFinished("RightWrist", _rightWristTextureIdx, _rightWristOffsetIdx);
    }

    private void SetEffectAnimation(string name, AttackAction action1, AttackAction? action2 = null, int action2SpriteNumber = 0, int action2Offset = 0)
    {
        if (name.Equals(_effect))
        {
            return;
        }
        _effect = name;
        var sprites = _spriteLoader.Load(name);
        UpdateNodeAnimationLibrary(action1.ToString(), _attackEffectTextureIdx, _attackEffectOffsetIdx, sprites);
        if (action2 == null)
        {
            return;
        }
        var tmp = new OffsetTexture[action2SpriteNumber * DirectionNumber];
        Array.Copy(sprites, action2Offset, tmp, 0, tmp.Length);
        UpdateNodeAnimationLibrary(action2.Value.ToString(), _attackEffectTextureIdx, _attackEffectOffsetIdx, tmp);
    }

    public void SetEffectAnimationIfNamePresent(string name, AttackAction action)
    {
        if (string.IsNullOrEmpty(name))
        {
            GetNode<Sprite2D>("../AttackEffect").Visible = false;
            return;
        }
        switch (action)
        {
            case AttackAction.Kick:
            case AttackAction.Punch:
                SetEffectAnimation(name, AttackAction.Kick, AttackAction.Punch, PunchSpriteNumber, KickSpriteNumber * DirectionNumber);
                break;
            case AttackAction.Blade1H:
            case AttackAction.Blade2H:
                SetEffectAnimation(name, AttackAction.Sword1H, AttackAction.Blade2H, Blade2HSpriteNumber, Sword1HSpriteNumber * DirectionNumber);
                break;
            case AttackAction.Sword1H:
            case AttackAction.Sword2H:
                SetEffectAnimation(name, AttackAction.Sword1H, AttackAction.Sword2H, Sword2HSpriteNumber, (Sword1HSpriteNumber + Blade2HSpriteNumber) * DirectionNumber);
                break;
            case AttackAction.Axe:
            case AttackAction.Spear:
                SetEffectAnimation(name, AttackAction.Axe);
                break;
            case AttackAction.Throw:
                SetEffectAnimation(name, AttackAction.Sword1H);
                break;
            case AttackAction.Bow:
                SetEffectAnimation(name, AttackAction.Bow);
                break;
        }
        GetNode<Sprite2D>("../AttackEffect").Visible = true;
    }
}