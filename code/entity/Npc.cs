using System.Threading;
using System.Threading.Tasks;
using Godot;
using NLog;
using QnClient.code.entity.@event;
using QnClient.code.message;
using QnClient.code.util;

namespace QnClient.code.entity;

public partial class Npc : AbstractCreature, INpcMessageHandler
{
    private NpcAnimationPlayer _animationPlayer;
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private Effect _effect;
    private Label _questMarker;
    
    public override void _Ready()
    {
        base._Ready();
        _effect = GetNode<Effect>("Effect");
        _animationPlayer = GetNode<NpcAnimationPlayer>("AnimationPlayer");
        Visible = false;
        ZIndex = 2;
        _questMarker = GetNode<Label>("QuestMarker");
        _questMarker.Position = new Vector2(18, -50) - _questMarker.GetSize() / 2;
        _questMarker.Visible = false;
    }

    protected override bool IsPlayer => false;

    public void Initialize(NpcSnapshot snapshot)
    {
        _animationPlayer.Initialize(snapshot.Shape, snapshot.Animate);
        base.Initialize(snapshot);
        switch (snapshot.NpcState)
        {
            case NpcState.Move:
                Move(snapshot.DurationMillis, snapshot.Direction, snapshot.ElapsedMillis);
                break;
            default:
                _animationPlayer.Play(snapshot.NpcState, snapshot.Direction, snapshot.ElapsedMillis);
                break;
        }
        EmitEvent(new EntityChangeCoordinateEvent(this));
        Visible = true;
    }

    private Vector2 ComputeProjectileStartPoint(CreatureDirection direction)
    {
        return Position + VectorUtil.DefaultTextureOffset;
    }

    public void FireProjectile(long targetId, string sprite, int flyMillis)
    {
        FireProjectile(targetId, sprite, flyMillis, ComputeProjectileStartPoint);
    }

    public void ActivateEffect(float seconds)
    {
        _effect.Show(seconds);
    }

    private Vector2 ComputeVelocity(CreatureDirection direction, float duration)
    {
        return VectorUtil.VelocityUnit(direction) / duration;
    }
    

    private void Move(int durationMillis, CreatureDirection direction, int elapsedMillis = 0)
    {
        var durationSec = (float)durationMillis / 1000;
        var length = _animationPlayer.MoveAnimationLength;
        var playSpeed = length / durationSec;
        _animationPlayer.PlayMove(direction, elapsedMillis, playSpeed);
        if (length > durationSec)
            length = durationSec;
        Mover = new EntityMover(this, length, ComputeVelocity(direction, length), (float) elapsedMillis / 1000);
    }

    public void Move(MoveMessage message)
    {
        Position = message.Start.ToPosition();
        Move(message.DurationMillis, message.Direction);
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
        Position = message.Coordinate.ToPosition();
        EmitEvent(new EntityChangeCoordinateEvent(this));
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