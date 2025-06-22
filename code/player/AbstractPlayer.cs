using Godot;
using QnClient.code.creature;
using QnClient.code.entity;
using QnClient.code.util;

namespace QnClient.code.player;

public partial class AbstractPlayer : Node2D, IEntity
{
     private PlayerAnimationPlayer _animationPlayer;
    /*private string? _hatName;
    private string? _legName;
    private string? _bootName;
    private string? _leftWrist;
    private string? _rightWrist;
    private string? _chest;
    private string? _armor;
    private string? _weapon;*/
    public override void _Ready()
    {
        _animationPlayer = GetNode<PlayerAnimationPlayer>("AnimationPlayer");
        _animationPlayer.InitializeAnimations(true);
        Position = new Vector2I(181, 238).ToPosition();
    }
    
    public PlayerAnimationPlayer AnimationPlayer => _animationPlayer;
    
    public CreatureDirection Direction { get; set; }
    
    private void PutOnHat(string name)
    {
        GetNode<Sprite2D>("Hat").Visible = true;
        _animationPlayer.SetHatAnimation(name);
    }

    private void HideHat()
    {
        GetNode<Sprite2D>("Hat").Visible = false;
    }
    
    private void PutOnLeg(string name)
    {
        GetNode<Sprite2D>("Leg").Visible = true;
        _animationPlayer.SetLegAnimation(name);
    }

    private void HideLeg()
    {
        GetNode<Sprite2D>("Leg").Visible = false;
    }
    
    private void PutOnBoot(string name)
    {
        GetNode<Sprite2D>("Boot").Visible = true;
        _animationPlayer.SetBootAnimation(name);
    }

    private void HideBoot()
    {
        GetNode<Sprite2D>("Boot").Visible = false;
    }
    
    private void PutOnWrist(string l, string r)
    {
        GetNode<Sprite2D>("LeftWrist").Visible = true;
        GetNode<Sprite2D>("RightWrist").Visible = true;
        _animationPlayer.SetWristAnimation(l, r);
    }
    
    private void HideWrist()
    {
        GetNode<Sprite2D>("LeftWrist").Visible = false;
        GetNode<Sprite2D>("RightWrist").Visible = false;
    }
    
    private void PutOnVest(string name)
    {
        GetNode<Sprite2D>("Vest").Visible = true;
        _animationPlayer.SetVestAnimation(name);
    }

    private void HideVest()
    {
        GetNode<Sprite2D>("Vest").Visible = false;
    }
    
    private void PutOnArmor(string name)
    {
        GetNode<Sprite2D>("Armor").Visible = true;
        _animationPlayer.SetVestAnimation(name);
    }

    private void HideArmor()
    {
        GetNode<Sprite2D>("Armor").Visible = false;
    }
    
    private void PutOnHair(string name)
    {
        GetNode<Sprite2D>("Hair").Visible = true;
        _animationPlayer.SetHairAnimation(name);
    }

    private void HideHair()
    {
        GetNode<Sprite2D>("Hair").Visible = false;
    }
    
    private void PutOnWeapon(string name)
    {
        GetNode<Sprite2D>("Weapon").Visible = true;
        _animationPlayer.SetBladeAnimation(name);
    }

    private void HideWeapon()
    {
        GetNode<Sprite2D>("Weapon").Visible = false;
    }

    public string EntityName { get; protected set; }
    public long Id { get; protected set; }
    public Vector2I Coordinate => Position.ToCoordinate();
}