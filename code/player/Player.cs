

using System;
using Godot;
using QnClient.code.entity;
using QnClient.code.entity.@event;
using QnClient.code.message;
using QnClient.code.util;

namespace QnClient.code.player;

public partial class Player : AbstractPlayer, IPlayerMessageHandler
{
    public override void _Ready()
    {
        base._Ready();
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
        EmitEvent(new EntityChangeCoordinateEvent(this));
    }

    public void Move(MoveMessage message)
    {
        if (message.Action == null)
            throw new NotSupportedException();
        Position = message.From.ToPosition();
        PlayMoveAnimation(message.Action.Value, message.Direction);
        CreateMover(message.Action.Value, message.Direction);
    }

    private void OnAnimationDone(StringName name)
    {
        if (name.ToString().Contains("Walk"))
        {
            var dirString = name.ToString().Split("/")[1];
            Enum.TryParse<CreatureDirection>(dirString, out var dir);
            AnimationPlayer.PlayIdle(dir);
        }
    }


    public void Initialize(PlayerSnapshot snapshot)
    {
        AnimationPlayer.InitializeAnimations(snapshot.Male);
        AnimationPlayer.AnimationFinished += OnAnimationDone;
        base.Initialize(snapshot);
        switch (snapshot.PlayerState)
        {
            case PlayerState.Move:
                CreateMover(snapshot.MoveAction, snapshot.Direction, snapshot.ElapsedMillis);
                PlayMoveAnimation(snapshot.MoveAction, snapshot.Direction, snapshot.ElapsedMillis);
                break;
            case PlayerState.Hello:
                AnimationPlayer.PlayHelloFrom(snapshot.Direction, snapshot.ElapsedMillis);
                break;
            case PlayerState.Hurt:
                AnimationPlayer.PlayHurtFrom(snapshot.Direction, snapshot.ElapsedMillis);
                break;
            case PlayerState.Die:
                AnimationPlayer.PlayDieFrom(snapshot.Direction, snapshot.ElapsedMillis);
                break;
            case PlayerState.Sit:
                AnimationPlayer.PlaySitFrom(snapshot.Direction, snapshot.ElapsedMillis);
                break;
            case PlayerState.StandUp:
                AnimationPlayer.PlayStandUpFrom(snapshot.Direction, snapshot.ElapsedMillis);
                break;
            case PlayerState.Idle:
                AnimationPlayer.PlayIdleFrom(snapshot.Direction, snapshot.ElapsedMillis);
                break;
            case PlayerState.FightStand:
                AnimationPlayer.PlayFightStandFrom(snapshot.Direction, snapshot.ElapsedMillis);
                break;
        }
        Visible = true;
        foreach (var snapshotEquipMessage in snapshot.EquipMessages)
        {
            snapshotEquipMessage.Accept(this);
        }
        EmitEvent(new EntityChangeCoordinateEvent(this));
        GD.Print("Created player " + Id);
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
                AnimationPlayer.PlayWalk(direction);
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
        EmitEvent(new EntityChangeCoordinateEvent(this));
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