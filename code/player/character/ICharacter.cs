using Godot;
using QnClient.code.entity;
using QnClient.code.map;
using QnClient.code.network;
using QnClient.code.player.character.kungfu;

namespace QnClient.code.player.character;

public interface ICharacter : ICreature
{
    void ChangeState(ICharacterState state);

    PlayerAnimationPlayer AnimationPlayer
    {
        get;
    }
    
    IMap Map
    {
        get;
    }
    
    Vector2 Position { get; set; }

    Vector2 GetLocalMousePosition();
    
    FootKungFu? FootKungFu { get; }
    
    Connection Connection { get; }
}