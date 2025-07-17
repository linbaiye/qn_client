using System;
using Godot;
using QnClient.code.entity.@event;
using QnClient.code.message;
using QnClient.code.util;
using TextBubble = QnClient.code.hud.TextBubble;

namespace QnClient.code.entity;

public abstract partial class AbstractCreature : AbstractEntity, ICreature
{
    private string _name;

    public string EntityName => _name;

    private TextBubble _textBubble;
    
    private BodySprite _bodySprite;

    private LifeBar _lifeBar;

    private AbstractAnimationPlayer _animationPlayer;
    
    public event Action<long>? AttackTriggered;
    public event Action<ShootEvent>? ShootEvent;
    
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
        label.Position = new Vector2(18, -20) - label.GetTextSize(name) / 2;
        //label.Position = (IsPlayer ? new Vector2(18, -20) : new Vector2(16, -20)) - label.GetTextSize(name) / 2;
        label.Visible = false;
    }
    
    protected abstract bool IsPlayer { get; }

    private void OnMouseEntered()
    {
        var label = GetNode<Label>("Name");
        // var offset = _animationPlayer.BodyOffset;
        // var size = _animationPlayer.BodySize;
        // if (Humanoid)
        //     offset += new Vector2(size.X / 2, 12);
        // else
        //     offset += new Vector2(size.X / 2,  5);
        // label.Position = new Vector2(offset.X - label.Size.X / 2,  offset.Y);
        label.Visible = true;
    }


    private ShaderMaterial CreateShadowShader()
    {
        var shader = ResourceLoader.Load<Shader>("res://shader/Shadow.gdshader");
        var shaderMaterial = new ShaderMaterial();
        shaderMaterial.Shader = shader;
        return shaderMaterial;
    }
    

    protected void Initialize(long id, Vector2I coordinate, string name)
    {
        Id = id;
        var bodySprite = GetNode<BodySprite>("Body");
        bodySprite.Material = CreateShadowShader();
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

    public Vector2 ProjectileAimPoint
    {
        get
        {
            var offset = _bodySprite.Offset;
            var texture = _bodySprite.GetTexture();
            var point = offset + new Vector2(Math.Min(15, texture.GetSize().X / 2), texture.GetSize().Y / 2);
            return point + Position;
        }
    }

    private Vector2 CenterXy
    {
        get
        {
            var offset = _animationPlayer.BodyOffset;
            var size = _animationPlayer.BodySize;
            return offset + new Vector2(size.X / 2, 0);
        }
    }
    
    protected void FireProjectile(long targetId, string sprite, int flyMillis, Func<CreatureDirection, Vector2> startPoint)
    {
        var ani = _animationPlayer.CurrentAnimation;
        if (string.IsNullOrEmpty(ani))
            return;
        CreatureDirection direction = Enum.Parse<CreatureDirection>(ani.Split("/")[1]);
        ShootEvent?.Invoke(new ShootEvent(targetId, startPoint.Invoke(direction), sprite, flyMillis));
    }

    public void ShowLifeBar(int percent)
    {
        _lifeBar.Show(percent);
    }

    public void Say(CreatureSayMessage sayMessage)
    {
        _textBubble.Display(sayMessage.Text, CenterXy);
    }
}