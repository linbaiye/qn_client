using System;
using Godot;
using QnClient.code.hud;

namespace QnClient.code.entity;

public partial class BodySprite: Sprite2D
{
    private MouseArea _mouseArea;

    public event Action? MouseEntered;
    
    public event Action? MouseExited;
    
    public event Action? AttackInvoked;
    
    public event Action? Clicked;

    private bool _hovering;
    
    public override void _Ready()
    {
        _mouseArea = GetNode<MouseArea>("MouseArea");
        _mouseArea.MouseEntered += OnMouseEntered;
        _mouseArea.MouseExited += OnMouseExited;
        _mouseArea.Clicked += () => Clicked?.Invoke();
        _mouseArea.AttackInvoked += () => AttackInvoked?.Invoke();
    }

    private void OnMouseEntered()
    {
        _hovering = true;
        MouseEntered?.Invoke();
    }
    
    private void OnMouseExited()
    {
        _hovering = false;
        MouseExited?.Invoke();
    }
    
    public MouseArea MouseArea => _mouseArea;

    public bool HasMouseHover()
    {
        return _hovering;
    }
    
    public void AttachShadowShader()
    {
        var shader = ResourceLoader.Load<Shader>("res://shader/Shadow.gdshader");
        var shaderMaterial = new ShaderMaterial();
        shaderMaterial.Shader = shader;
        Material = shaderMaterial;
    }
}