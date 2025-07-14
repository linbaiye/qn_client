using System;
using Godot;
using QnClient.code.entity.@event;
using QnClient.code.message;
using QnClient.code.util;
using TextBubble = QnClient.code.hud.TextBubble;

namespace QnClient.code.entity;

public abstract partial class AbstractCreature : Node2D, IEntity
{
    public event Action<IEntityEvent>? OnEntityEvent;

    private string _name;
    

    public string EntityName => _name;

    private TextBubble _textBubble;
    
    private BodySprite _bodySprite;

    private LifeBar _lifeBar;

    private AbstractAnimationPlayer _animationPlayer;
    
    public event Action<long>? AttackTriggered;
    
    public override void _Ready()
    {
        _textBubble = GetNode<TextBubble>("TextBubble");
        _bodySprite = GetNode<BodySprite>("Body");
        _lifeBar = GetNode<LifeBar>("LifeBar");
        _animationPlayer = GetNode<AbstractAnimationPlayer>("AnimationPlayer");
    }
    
    
    private void InitializeViewName(string name)
    {
        _name = name;
        var label = GetNode<Label>("Name");
        label.SetText(name);
        label.Visible = false;
    }

    public long Id { get; private set; }
    public Vector2I Coordinate => Position.ToCoordinate();
    
    public abstract void HandleEntityMessage(IEntityMessage message);

    public void EmitEvent(IEntityEvent entityEvent)
    {
        OnEntityEvent?.Invoke(entityEvent);
    }
    
    protected abstract bool Humanoid { get; }

    private void OnMouseEntered()
    {
        var label = GetNode<Label>("Name");
        var offset = _animationPlayer.BodyOffset;
        var size = _animationPlayer.BodySize;
        if (Humanoid)
            offset += new Vector2(size.X / 2, 12);
        else
            offset += new Vector2(size.X / 2,  5);
        label.Position = new Vector2(offset.X - label.Size.X / 2,  offset.Y);
        /*//var p = new Vector2(positionTexture.Value.OriginalSize.X / 2 + positionTexture.Value.Offset.X, positionTexture.Value.Offset.Y);
        var idle = _animationPlayer.GetCurrentDirectionIdle();
        var size = label.Size;
        var anchor = new Vector2(idle.Offset.X,  idle.Offset.Y + idle.OriginalSize.Y / 2);
        label.Position = new Vector2(anchor.X,  anchor.Y);*/
        label.Visible = true;
    }
    

    protected void Initialize(long id, Vector2I coordinate, string name)
    {
        Id = id;
        var bodySprite = GetNode<BodySprite>("Body");
        bodySprite.MouseEntered += OnMouseEntered;
        bodySprite.MouseExited += () => GetNode<Label>("Name").Visible = false;
        bodySprite.AttackInvoked += () => AttackTriggered?.Invoke(Id);
        Position = coordinate.ToPosition();
        InitializeViewName(name);
    }

    protected void Initialize(AbstractCreatureSnapshot snapshot)
    {
        Initialize(snapshot.Id, snapshot.Coordinate, snapshot.Name);
    }

    public Vector2 CenterPoint => _bodySprite.Center + Position;
        

    private Vector2 CenterXy
    {
        get
        {
            var offset = _animationPlayer.BodyOffset;
            var size = _animationPlayer.BodySize;
            return offset + new Vector2(size.X / 2, 0);
        }
    }

    public void ShowLifeBar(int percent)
    {
        _lifeBar.Show(percent, CenterXy);
    }

    public void Say(CreatureSayMessage sayMessage)
    {
        _textBubble.Display(sayMessage.Text, CenterXy);
    }
}