using System;
using Godot;
using QnClient.code.entity;
using QnClient.code.message;
using QnClient.code.ui;

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
    
    public PlayerAnimationPlayer AnimationPlayer => _animationPlayer;
    
    private void PutOnHat(string spritePrefix, int color)
    {
        _hat.TextureChanged += () => DyeIfHasColor(_hat, color);
        _animationPlayer.SetHatAnimation(spritePrefix);
        _hat.Visible = true;
    }

    private void HideHat()
    {
        _hat.Visible = false;
    }
    
    private void PutOnLeg(string spritePrefix, int color)
    {
        _leg.TextureChanged += () => DyeIfHasColor(_leg, color);
        _animationPlayer.SetLegAnimation(spritePrefix);
        _leg.Visible = true;
    }

    private void HideLeg()
    {
        _leg.Visible = false;
    }
    
    private void PutOnBoot(string spritePrefix, int color)
    {
        _boot.TextureChanged += () => DyeIfHasColor(_boot, color);
        _animationPlayer.SetBootAnimation(spritePrefix);
        _boot.Visible = true;
    }

    private void HideBoot()
    {
        _boot.Visible = false;
    }
    
    private void PutOnWrist(string l, string r, int color)
    {
        _leftWrist.TextureChanged += () => DyeIfHasColor(_leftWrist, color);
        _rightWrist.TextureChanged += () => DyeIfHasColor(_rightWrist, color);
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
        _vest.TextureChanged += () => DyeIfHasColor(_vest, color);
        _animationPlayer.SetVestAnimation(spritePrefix);
        _vest.Visible = true;
    }

    private void HideVest()
    {
        _vest.Visible = false;
    }
    
    private void PutOnArmor(string spritePrefix, int color)
    {
        _armor.TextureChanged += () => DyeIfHasColor(_armor, color);
        _animationPlayer.SetArmorAnimation(spritePrefix);
        _armor.Visible = true;
    }

    private void HideArmor()
    {
        _armor.Visible = false;
    }
    
    
    private void PutOnHair(string spritePrefix, int color)
    {
        _hair.TextureChanged += () => DyeIfHasColor(_hair, color);
        DyeIfHasColor(_hair, color);
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

    private void HideWeapon()
    {
        _weapon.Visible = false;
    }

    private void DyeIfHasColor(Sprite2D sprite, int color)
    {
        if (color <= 0)
            return;
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
}