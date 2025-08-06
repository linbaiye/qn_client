

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
        Mover = null;
        DoSetPositionState(message.Coordinate, message.State, message.Direction);
    }

    public void Move(MoveMessage message)
    {
        if (message.Action == null)
            throw new NotSupportedException();
        Position = message.Start.ToPosition();
        EmitEvent(new EntityChangeCoordinateEvent(this));
        CreateMover(message.Action.Value, message.Direction, message.StartMillis);
        PlayMoveAnimation(message.Action.Value, message.Direction, message.StartMillis);
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
            default:
                PlayStateAnimation(snapshot.PlayerState, snapshot.Direction, snapshot.ElapsedMillis);
                break;
        }
        Visible = true;
        foreach (var snapshotEquipMessage in snapshot.EquipMessages)
        {
            snapshotEquipMessage.Accept(this);
        }
        EmitEvent(new EntityChangeCoordinateEvent(this));
    }

    public void ChangeState(Vector2I coor, PlayerState newState, CreatureDirection direction)
    {
        Mover = null;
        if (newState != PlayerState.Hurt)
            Position = coor.ToPosition();
        EmitEvent(new EntityChangeCoordinateEvent(this));
        PlayStateAnimation(newState, direction);
    }
    
    public void Attack(AttackAction action, CreatureDirection direction, string effect)
    {
        Mover = null;
        PlayAttackAnimation(action, direction, effect);
    }


    private void CreateMover(MoveAction action, CreatureDirection direction, int elapsedMillis = 0)
    {
        var duration = VectorUtil.GetMoveDuration(action);
        var v = VectorUtil.VelocityUnit(direction) / duration;
        Mover = new EntityMover(this, duration, v, (float)elapsedMillis / 1000);
    }

    public override void _PhysicsProcess(double delta)
    {
        UpdateMover(delta);
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

    public void Remove()
    {
        EmitEvent(new DeletedEvent(this));
        QueueFree();
    }
}