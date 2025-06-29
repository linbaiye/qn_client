using Godot;
using NLog;
using QnClient.code.entity.@event;
using QnClient.code.message;
using QnClient.code.util;

namespace QnClient.code.entity;

public partial class Npc : AbstractCreature, INpcMessageHandler
{
    private MonsterAnimationPlayer _animationPlayer;
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    public override void _Ready()
    {
        base._Ready();
        _animationPlayer = GetNode<MonsterAnimationPlayer>("AnimationPlayer");
        Visible = false;
        ZIndex = 2;
    }

    public void Initialize(NpcSnapshot snapshot)
    {
        _animationPlayer.Initialize(snapshot.Shape, snapshot.Animate);
        base.Initialize(snapshot);
        switch (snapshot.NpcState)
        {
            case NpcState.Move:
                CreateMover(snapshot.Direction, (float)snapshot.ElapsedMillis / 1000);
                _animationPlayer.PlayMove(snapshot.Direction, snapshot.ElapsedMillis);
                break;
            case NpcState.Idle:
                _animationPlayer.PlayIdle(snapshot.Direction, snapshot.ElapsedMillis);
                break;
            case NpcState.Attack:
                _animationPlayer.PlayAttack(snapshot.Direction, snapshot.ElapsedMillis);
                break;
            case NpcState.Die:
                _animationPlayer.PlayDie(snapshot.Direction, snapshot.ElapsedMillis);
                break;
            case NpcState.Hurt:
                _animationPlayer.PlayHurt(snapshot.Direction, snapshot.ElapsedMillis);
                break;
        }
        Visible = true;
        EmitEvent(new EntityCoordinateEvent(this));
    }

    private Vector2 ComputeVelocity(CreatureDirection direction)
    {
        return VectorUtil.VelocityUnit(direction) / _animationPlayer.MoveAnimationLength;
    }



    private void CreateMover(CreatureDirection direction, float elapsedSeconds = 0)
    {
        var length = _animationPlayer.MoveAnimationLength;
        Mover = new EntityMover(this, length, ComputeVelocity(direction), elapsedSeconds);
    }

    public void Move(MoveMessage message)
    {
        Position = message.From.ToPosition();
        _animationPlayer.PlayMove(message.Direction);
        CreateMover(message.Direction);
    }

    public void SetPosition(SetPositionMessage message)
    {
        Position = message.Coordinate.ToPosition();
        _animationPlayer.PlayIdle(message.Direction);
        EmitEvent(new EntityCoordinateEvent(this));
    }

    public void ChangeState(NpcChangeStateMessage message)
    {
        _animationPlayer.Play(message.State,  message.Direction);
    }

    private EntityMover? Mover { get; set; }
    
    public override void _PhysicsProcess(double delta)
    {
        if (Mover == null || !Mover.PhysicProcess(delta))
            return;
        EmitEvent(new EntityCoordinateEvent(this));
        Mover = null;
    }

    public override void HandleEntityMessage(IEntityMessage message)
    {
        if (message is INpcMessage npcMessage)
        {
            npcMessage.Accept(this);
        }
    }
    
    public static Npc Create()
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://scene/npc.tscn");
        return scene.Instantiate<Npc>();
    }
}