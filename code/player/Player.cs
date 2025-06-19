using System;
using Godot;
using QnClient.code.creature;
using QnClient.code.util;

namespace QnClient.code.player;

public partial class Player : Node2D
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
        _animationPlayer.PlayIdle(CreatureDirection.Down);
        Position = new Vector2I(181, 238).ToPosition();
    }
    
    public AnimationPlayer AnimationPlayer => _animationPlayer;
    
    public delegate void TestPlay(CreatureDirection direction);

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

    /*public override void _UnhandledInput(InputEvent @event)
    {
        TestPlay player = _animationPlayer.PlayBlade2HAttack;
        if (@event is InputEventKey eventKey && eventKey.Pressed)
        {
            if (eventKey.Keycode == Key.H)
            {
                if (_hatName == null)
                {
                    _hatName = "v16";
                    PutOnHat(_hatName);
                }
                else
                {
                    _hatName = null;
                    HideHat();
                }
            }
            else if (eventKey.Keycode == Key.L)
            {
                if (_legName == null)
                {
                    _legName = "r1";
                    PutOnLeg(_legName);
                }
                else
                {
                    _legName = null;
                    HideLeg();
                }
            } 
            else if (eventKey.Keycode == Key.B)
            {
                if (_bootName== null)
                {
                    _bootName = "q1";
                    PutOnBoot(_bootName);
                }
                else
                {
                    _bootName= null;
                    HideBoot();
                }
            }
            else if (eventKey.Keycode == Key.W)
            {
                if (_leftWrist == null)
                {
                    _leftWrist = "o1";
                    _rightWrist = "s1";
                    PutOnWrist(_leftWrist, _rightWrist);
                }
                else
                {
                    _leftWrist = null;
                    _rightWrist = null;
                    HideWrist();
                }
            }
            else if (eventKey.Keycode == Key.C)
            {
                if (_chest == null)
                {
                    _chest = "p1";
                    PutOnVest(_chest);
                }
                else
                {
                    HideVest();
                }
            }
            else if (eventKey.Keycode == Key.A)
            {
                if (_armor == null)
                {
                    _armor = "t1";
                    PutOnArmor(_armor);
                }
                else
                {
                    HideArmor();
                }
            }
            else if (eventKey.Keycode == Key.R)
            {
                if (_armor == null)
                {
                    _armor = "u1";
                    PutOnHair(_armor);
                }
                else
                {
                    HideHair();
                }
            }
            else if (eventKey.Keycode == Key.E)
            {
                if (_weapon == null)
                {
                    _weapon = "w1";
                    PutOnWeapon(_weapon);
                    _animationPlayer.SetBladeEffectAnimation("_232");
                }
                else
                {
                    HideWeapon();
                }
            }
            else if (eventKey.Keycode >= Key.Key1 && eventKey.Keycode <= Key.Key8)
            {
                player.Invoke((CreatureDirection)((int)eventKey.Keycode - (int)Key.Key1));
            }
        }
    }*/
}