using System;
using Godot;
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
    
    public event Action<long>? AttackTriggered;
    
    public override void _Ready()
    {
        _textBubble = GetNode<TextBubble>("TextBubble");
        _bodySprite = GetNode<BodySprite>("Body");
        _lifeBar = GetNode<LifeBar>("LifeBar");
    }
    
    private void SetViewName(string name)
    {
        _name = name;
        var label = GetNode<Label>("Name");
        label.Text = name;
        label.Visible = false;
    }

    public long Id { get; private set; }
    public Vector2I Coordinate => Position.ToCoordinate();
    
    public abstract void HandleEntityMessage(IEntityMessage message);

    public void EmitEvent(IEntityEvent entityEvent)
    {
        OnEntityEvent?.Invoke(entityEvent);
    }

    private void OnMouseEntered()
    {
        var label = GetNode<Label>("Name");
        var xCenterY = _bodySprite.Center;
        var size = label.Size;
        label.Position = new Vector2(xCenterY.X - size.X / 2,  xCenterY.Y - 20);
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
        SetViewName(name);
    }

    protected void Initialize(AbstractCreatureSnapshot snapshot)
    {
        Initialize(snapshot.Id, snapshot.Coordinate, snapshot.Name);
    }

    public void ShowLifeBar(int percent)
    {
        _lifeBar.Show(percent, _bodySprite.XCenterY);
    }

    public void Say(CreatureSayMessage sayMessage)
    {
        _textBubble.Display(sayMessage.Text, _bodySprite.XCenterY);
    }
}