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

    protected override bool Humanoid => false;

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
            default:
                _animationPlayer.Play(snapshot.NpcState, snapshot.Direction, snapshot.ElapsedMillis);
                break;
        }
        Visible = true;
        EmitEvent(new EntityChangeCoordinateEvent(this));
    }

    private Vector2 ComputeProjectileStartPoint(CreatureDirection direction)
    {
        return Position + VectorUtil.DefaultTextureOffset;
    }

    public void FireProjectile(long targetId, string sprite, int flyMillis)
    {
        FireProjectile(targetId, sprite, flyMillis, ComputeProjectileStartPoint);
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
        Position = message.Start.ToPosition();
        _animationPlayer.PlayMove(message.Direction);
        CreateMover(message.Direction);
        EmitEvent(new EntityChangeCoordinateEvent(this));
    }

    public void SetPosition(SetPositionMessage message)
    {
        Mover = null;
        Position = message.Coordinate.ToPosition();
        _animationPlayer.PlayIdle(message.Direction);
        EmitEvent(new EntityChangeCoordinateEvent(this));
    }

    public void ChangeState(NpcChangeStateMessage message)
    {
        Mover = null;
        _animationPlayer.Play(message.State,  message.Direction);
    }

    private EntityMover? Mover { get; set; }
    
    public override void _PhysicsProcess(double delta)
    {
        if (Mover == null || !Mover.PhysicProcess(delta))
            return;
        EmitEvent(new EntityChangeCoordinateEvent(this));
        Mover = null;
    }

    public override void HandleEntityMessage(IEntityMessage message)
    {
        if (message is INpcMessage npcMessage)
        {
            npcMessage.Accept(this);
        }
    }
    
    public void Remove()
    {
        EmitEvent(new DeletedEvent(this));
        QueueFree();
    }
    
    public static Npc Create()
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://scene/npc.tscn");
        return scene.Instantiate<Npc>();
    }
}