using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using NLog;
using QnClient.code.entity.@event;
using QnClient.code.message;
using QnClient.code.util;

namespace QnClient.code.entity;

public partial class DynamicObject : AbstractEntity, IDynamicObjectMessageHandler
{

    private BodySprite _bodySprite;

    private DynamicObjectAnimationPlayer _animationPlayer;

    private readonly ISet<Vector2I> _coordinates = new HashSet<Vector2I>();
    private LifeBar _lifeBar;
    public event Action<long>? AttackTriggered;
    
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private int _aniId2;

    private bool _liftCoordinates ;

    private Label _name;
    
    public override void _Ready()
    {
        base._Ready();
        _animationPlayer = GetNode<DynamicObjectAnimationPlayer>("AnimationPlayer");
        _lifeBar = GetNode<LifeBar>("LifeBar");
        _bodySprite = GetNode<BodySprite>("Body");
        _name = GetNode<Label>("Name");
        ZIndex = 2;
        _name.Visible = false;
        _bodySprite.MouseEntered += () => _name.Visible = true;
        _bodySprite.MouseExited += () => _name.Visible = false;
        Visible = false;
    }


    public override void HandleEntityMessage(IEntityMessage message)
    {
        if (message is IDynamicObjectMessage objectMessage)
            objectMessage.Accept(this);
    }

    public async void Initialize(DynamicObjectSnapshot snapshot)
    {
        Position = snapshot.Coordinate.ToPosition();
        Id = snapshot.Id;
        var offsetTexture = _animationPlayer.Initialize(snapshot.Shape, snapshot.Animations, snapshot.Offset);
        foreach (var c in snapshot.Coordinates)
        {
            _coordinates.Add(c);
        }
        _coordinates.Add(snapshot.Coordinate);
        _animationPlayer.PlayId(snapshot.AnimateId, snapshot.Elapsed);
        if (snapshot.Occupying)
            EmitEvent(new EntityChangeCoordinateEvent(this));
        _bodySprite.AttackInvoked += () => AttackTriggered?.Invoke(Id);
        _animationPlayer.AnimationFinished += OnDone;
        var offset = offsetTexture.Offset + offsetTexture.OriginalSize / 2 + new Vector2(16,0);
        _name.Text = snapshot.Name;
        _name.Position = offset - _name.GetTextSize(snapshot.Name) / 2;
        _lifeBar.Position = offset - _lifeBar.Size / 2 - new Vector2(0, offsetTexture.OriginalSize.Y / 2);
        // Fixes flickering, why?
        await Task.Run(() => Thread.Sleep(30));
        Visible = true;
    }

    private void OnDone(StringName name)
    {
        if (_liftCoordinates)
        {
            _liftCoordinates = false;
            EmitEvent(new LiftCoordinatesEvent(this));
        }
        if (_aniId2 != 0)
            _animationPlayer.PlayId(_aniId2);
    }

    public void Shift(int id, int id2, bool liftCoordinates)
    {
        _aniId2 = id2;
        _liftCoordinates = liftCoordinates;
        _animationPlayer.PlayId(id);
    }

    public override bool HasMouseHover()
    {
        return _bodySprite.HasMouseHover();
    }

    public IEnumerable<Vector2I> Coordinates => _coordinates;
    
    public static DynamicObject Create()
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://scene/dynamic_object.tscn");
        return scene.Instantiate<DynamicObject>();
    }

    public void Remove()
    {
        EmitEvent(new DeletedEvent(this));
        QueueFree();
    }

    public void ShowLifeBar(int percent)
    {
        _lifeBar.Show(percent);
    }
}