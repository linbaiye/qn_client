

using System;
using Godot;
using QnClient.code.entity;
using QnClient.code.entity.@event;
using QnClient.code.message;
using QnClient.code.util;

namespace QnClient.code.player;

public partial class Player : AbstractPlayer, IPlayerMessageHandler
{

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

    public override void _Ready()
    {
        Visible = false;
        ZIndex = 2;
    }

    public void SetPosition(SetPositionMessage message)
    {
        Position = message.Coordinate.ToPosition();
        switch (message.State)
        {
            case PlayerState.Idle:
                AnimationPlayer.PlayIdle(message.Direction);
                break;
            case PlayerState.FightStand:
                AnimationPlayer.PlayFightStand(message.Direction);
                break;
        }
        EmitEvent(new CoordinateChangedEvent(this));
    }

    public void Move(MoveMessage message)
    {
        if (message.Action == null)
            throw new NotSupportedException();
        Position = message.From.ToPosition();
        CreateMover(message.Action.Value, message.Direction);
    }


    public void Initialize(PlayerSnapshot snapshot)
    {
        base.Initialize(snapshot);
        AnimationPlayer.InitializeAnimations(snapshot.Male);
        switch (snapshot.StateSnapshot.State)
        {
            case PlayerState.Move:
                CreateMover(snapshot.StateSnapshot.MoveAction.Value, snapshot.Direction, snapshot.StateSnapshot.ElapsedMillis);
                PlayMoveAnimation(snapshot.StateSnapshot.MoveAction.Value, snapshot.Direction, snapshot.StateSnapshot.ElapsedMillis);
                break;
            case PlayerState.Hello:
                AnimationPlayer.PlayHelloFrom(snapshot.Direction, snapshot.StateSnapshot.ElapsedMillis);
                break;
            case PlayerState.Hurt:
                AnimationPlayer.PlayHurtFrom(snapshot.Direction, snapshot.StateSnapshot.ElapsedMillis);
                break;
            case PlayerState.Die:
                AnimationPlayer.PlayDieFrom(snapshot.Direction, snapshot.StateSnapshot.ElapsedMillis);
                break;
            case PlayerState.Sit:
                AnimationPlayer.PlaySitFrom(snapshot.Direction, snapshot.StateSnapshot.ElapsedMillis);
                break;
            case PlayerState.StandUp:
                AnimationPlayer.PlayStandUpFrom(snapshot.Direction, snapshot.StateSnapshot.ElapsedMillis);
                break;
            case PlayerState.Idle:
                AnimationPlayer.PlayIdleFrom(snapshot.Direction, snapshot.StateSnapshot.ElapsedMillis);
                break;
            case PlayerState.FightStand:
                AnimationPlayer.PlayFightStandFrom(snapshot.Direction, snapshot.StateSnapshot.ElapsedMillis);
                break;
        }
        Visible = true;
        EmitEvent(new CoordinateChangedEvent(this));
    }

    private void PlayMoveAnimation(MoveAction action, CreatureDirection direction, int startMillis = 0)
    {
        switch (action)
        {
            case MoveAction.Fly:
                AnimationPlayer.PlayFlyFrom(direction, startMillis);
                break;
            case MoveAction.Run:
                AnimationPlayer.PlayRunFrom(direction, startMillis);
                break;
            case MoveAction.Walk:
                AnimationPlayer.PlayWalkFrom(direction, startMillis);
                break;
            case MoveAction.FightWalk:
                AnimationPlayer.PlayFightWalkFrom(direction, startMillis);
                break;
        }
    }
    
    private EntityMover? Mover
    {
        get;
        set;
    }

    private void CreateMover(MoveAction action, CreatureDirection direction, int elapsedMillis = 0)
    {
        var duration = VectorUtil.GetMoveDuration(action);
        var v = VectorUtil.VelocityUnit(direction) / duration;
        Mover = new EntityMover(this, duration, v, (float)elapsedMillis / 1000);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Mover == null || !Mover.PhysicProcess(delta))
            return;
        EmitEvent(new CoordinateChangedEvent(this));
        Mover = null;
    }

    public override void HandleEntityMessage(IEntityMessage message)
    {
        if (message is IPlayerMessage playerMessage)
        {
            playerMessage.Accept(this);
        }
    }

    public static Player Create()
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://scene/player.tscn");
        return scene.Instantiate<Player>();
    }
}