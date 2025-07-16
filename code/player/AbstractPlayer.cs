using System;
using System.Collections.Generic;
using Godot;
using QnClient.code.entity;
using QnClient.code.entity.@event;
using QnClient.code.message;
using QnClient.code.util;
using BodySprite = QnClient.code.entity.BodySprite;

namespace QnClient.code.player;

public abstract partial class AbstractPlayer : AbstractCreature
{
    private PlayerAnimationPlayer _animationPlayer;
    private Sprite2D _hat;
    private Sprite2D _leg;
    private BodySprite _body;
    private Sprite2D _boot;
    private Sprite2D _leftWrist;
    private Sprite2D _rightWrist;
    private Sprite2D _vest;
    private Sprite2D _hair;
    private Sprite2D _armor;
    private Sprite2D _weapon;
    
    public override void _Ready()
    {
        base._Ready();
        _animationPlayer = GetNode<PlayerAnimationPlayer>("AnimationPlayer");
        _hat = GetNode<Sprite2D>("Hat");
        _leg = GetNode<Sprite2D>("Leg");
        _boot = GetNode<Sprite2D>("Boot");
        _body = GetNode<BodySprite>("Body");
        _leftWrist = GetNode<Sprite2D>("LeftWrist");
        _rightWrist = GetNode<Sprite2D>("RightWrist");
        _vest = GetNode<Sprite2D>("Vest");
        _hair = GetNode<Sprite2D>("Hair");
        _armor = GetNode<Sprite2D>("Armor");
        _weapon = GetNode<Sprite2D>("Weapon");
    }

    protected override bool Humanoid => true;
    
    public PlayerAnimationPlayer AnimationPlayer => _animationPlayer;
    
    private void PutOnHat(string spritePrefix, int color)
    {
        Dye(_hat, color);
        _animationPlayer.SetHatAnimation(spritePrefix);
        _hat.Visible = true;
    }

    private void HideHat()
    {
        _hat.Visible = false;
    }
    
    private void PutOnLeg(string spritePrefix, int color)
    {
        Dye(_leg, color);
        _animationPlayer.SetLegAnimation(spritePrefix);
        _leg.Visible = true;
    }

    private void HideLeg()
    {
        _leg.Visible = false;
    }
    
    private void PutOnBoot(string spritePrefix, int color)
    {
        Dye(_boot, color);
        _animationPlayer.SetBootAnimation(spritePrefix);
        _boot.Visible = true;
    }

    private void HideBoot()
    {
        _boot.Visible = false;
    }
    
    private void PutOnWrist(string l, string r, int color)
    {
        Dye(_leftWrist, color);
        Dye(_rightWrist, color);
        _animationPlayer.SetWristAnimation(l, r);
        _leftWrist.Visible = true;
        _rightWrist.Visible = true;
    }
    
    private void HideWrist()
    {
        _leftWrist.Visible = false;
        _rightWrist.Visible = false;
    }
    
    private void PutOnVest(string spritePrefix, int color)
    {
        Dye(_vest, color);
        _animationPlayer.SetVestAnimation(spritePrefix);
        _vest.Visible = true;
    }

    private void HideVest()
    {
        _vest.Visible = false;
    }
    
    private void PutOnArmor(string spritePrefix, int color)
    {
        Dye(_armor, color);
        _animationPlayer.SetArmorAnimation(spritePrefix);
        _armor.Visible = true;
    }

    private void HideArmor()
    {
        _armor.Visible = false;
    }
    
    
    private void PutOnHair(string spritePrefix, int color)
    {
        Dye(_hair, color);
        _animationPlayer.SetHairAnimation(spritePrefix);
        _hair.Visible = true;
    }

    private void HideHair()
    {
        _hair.Visible = false;
    }
    
    private void PutOnWeapon(string spritePrefix, WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.FistWeapon:
                _animationPlayer.SetFistWeaponAnimation(spritePrefix);
                break;
            case WeaponType.Axe:
                _animationPlayer.SetAxeAnimation(spritePrefix);
                break;
            case WeaponType.Spear:
                _animationPlayer.SetSpearAnimation(spritePrefix);
                break;
            case WeaponType.Sword:
                _animationPlayer.SetSwordAnimation(spritePrefix);
                break;
            case WeaponType.Bow:
                _animationPlayer.SetBowAnimation(spritePrefix);
                break;
            case WeaponType.Blade:
                _animationPlayer.SetBladeAnimation(spritePrefix);
                break;
            case WeaponType.Throw:
                _animationPlayer.SetThrowAnimation(spritePrefix);
                break;
        }
        _weapon.Visible = true;
    }

    public void Unequip(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.Weapon:
                HideWeapon();
                break;
            case EquipmentType.Wrist:
                HideWrist();
                break;
            case EquipmentType.Hair:
                HideHair();
                break;
            case EquipmentType.Armor:
                HideArmor();
                break;
            case EquipmentType.Hat:
                HideHat();
                break;
            case EquipmentType.Boot:
                HideBoot();
                break;
            case EquipmentType.Leg:
                HideLeg(); 
                break;
            case EquipmentType.Vest:
                HideVest();
                break;
        }
    }

    protected void DoSetPosition(Vector2I coor, PlayerState state, CreatureDirection direction)
    {
        Position = coor.ToPosition();
        switch (state)
        {
            case PlayerState.Idle:
                AnimationPlayer.PlayIdle(direction);
                break;
            case PlayerState.FightStand:
                AnimationPlayer.PlayFightStand(direction);
                break;
        }
        EmitEvent(new EntityChangeCoordinateEvent(this));
    }

    private void HideWeapon()
    {
        _weapon.Visible = false;
    }

    private void Dye(Sprite2D sprite, int color)
    {
        DyeShader.SetColor((ShaderMaterial)sprite.Material, color);
    }

    public void Equip(PlayerEquipMessage message)
    {
        switch (message.Type)
        {
            case EquipmentType.Boot:
                PutOnBoot(message.SpritePrefix, message.Color);
                break;
            case EquipmentType.Hat:
                PutOnHat(message.SpritePrefix, message.Color);
                break;
            case EquipmentType.Leg:
                PutOnLeg(message.SpritePrefix, message.Color);
                break;
            case EquipmentType.Hair:
                PutOnHair(message.SpritePrefix, message.Color);
                break;
            case EquipmentType.Armor:
                PutOnArmor(message.SpritePrefix, message.Color);
                break;
            case EquipmentType.Vest:
                PutOnVest(message.SpritePrefix, message.Color);
                break;
            case EquipmentType.Wrist:
                PutOnWrist(message.SpritePrefix, message.PairedSpritePrefix, message.Color);
                break;
            case EquipmentType.Weapon:
                PutOnWeapon(message.SpritePrefix, message.WeaponType);
                break;
        }
     }

    protected void PlayStateAnimation(PlayerState newState, CreatureDirection direction, int startMillis = 0)
    {
        switch (newState)
        {
            case PlayerState.Idle:
                _animationPlayer.PlayIdle(direction, startMillis);
                break;
            case PlayerState.FightStand:
                _animationPlayer.PlayFightStand(direction, startMillis);
                break;
            case PlayerState.Hurt:
                _animationPlayer.PlayHurt(direction, startMillis);
                break;
            case PlayerState.Die:
                _animationPlayer.PlayDie(direction, startMillis);
                break;
            case PlayerState.Sit:
                _animationPlayer.PlaySit(direction, startMillis);
                break;
            case PlayerState.StandUp:
                _animationPlayer.PlayStandUp(direction, startMillis);
                break;
            case PlayerState.Hello:
                _animationPlayer.PlayHello(direction, startMillis);
                break;
        }
    }

    private static readonly Dictionary<CreatureDirection, Vector2> ProjectileLetOffPoints = new()
    {
        { CreatureDirection.Up , new Vector2(6, 5) },
        { CreatureDirection.UpRight, new Vector2(40, 6) },
        { CreatureDirection.Right , new Vector2(40, 5) },
        { CreatureDirection.DownRight, new Vector2(30, 20) },
        { CreatureDirection.Down, new Vector2(15, 38) },
        { CreatureDirection.DownLeft, new Vector2(5, 30) },
        { CreatureDirection.Left, new Vector2(24, 15) },
        { CreatureDirection.UpLeft, new Vector2(20, 15) },
    };

    private bool _shoot;

    private AbstractCreature _target;

    public void Shoot(AbstractCreature target)
    {
        _shoot = true;
        var creatureDirection = Coordinate.GetDirection(target.Coordinate);
        _animationPlayer.PlayThrowAttack(creatureDirection);
        _target = target;
    }


    private Vector2 ComputeShootStartPoint(CreatureDirection direction)
    {
        return Position + _body.Offset + ProjectileLetOffPoints.GetValueOrDefault(direction, Vector2.Zero);
    }

    public void FireProjectile(long targetId, string sprite, int flyMillis)
    {
        FireProjectile(targetId, sprite, flyMillis, ComputeShootStartPoint);
    }
    
    public override void _Process(double delta)
    {
        if (!_shoot)
            return;
        var ani = _animationPlayer.CurrentAnimation;
        if (string.IsNullOrEmpty(ani))
            return;
        if (!ani.Split("/")[0].Equals(AttackAction.Throw.ToString()))
        {
            return;
        }
        if (_animationPlayer.CurrentAnimationPosition < 0.2f) 
            return;
        CreatureDirection direction = Enum.Parse<CreatureDirection>(ani.Split("/")[1]);
        var position = Position + _body.Offset + ProjectileLetOffPoints.GetValueOrDefault(direction, Vector2.Zero);
        //ShootEvent?.Invoke(new ShootEvent(_target.ProjectileAimPoint, position));
        _shoot = false;
        _target = null;
    }

    protected void PlayAttackAnimation(AttackAction action, CreatureDirection direction, string effect, int startMillis = 0)
    {
        _animationPlayer.SetEffectAnimationIfNamePresent(effect, action);
        switch (action)
        {
            case AttackAction.Punch:
                _animationPlayer.PlayPunch(direction, startMillis);
                break;
            case AttackAction.Kick:
                _animationPlayer.PlayKick(direction, startMillis);
                break;
            case AttackAction.Sword1H:
                _animationPlayer.PlaySword1HAttack(direction, startMillis);
                break;
            case AttackAction.Sword2H:
                _animationPlayer.PlaySword2HAttack(direction, startMillis);
                break;
            case AttackAction.Blade1H:
                _animationPlayer.PlayBlade1HAttack(direction, startMillis);
                break;
            case AttackAction.Blade2H:
                _animationPlayer.PlayBlade2HAttack(direction, startMillis);
                break;
            case AttackAction.Axe:
                _animationPlayer.PlayAxeAttack(direction, startMillis);
                break;
            case AttackAction.Spear:
                _animationPlayer.PlaySpearAttack(direction, startMillis);
                break;
            case AttackAction.Bow:
                _animationPlayer.PlayBowAttack(direction, startMillis);
                break;
            case AttackAction.Throw:
                _animationPlayer.PlayThrowAttack(direction, startMillis);
                break;
        }
    }

}