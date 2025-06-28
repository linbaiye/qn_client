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

    bool MovePressed { get; }

    Vector2 GetLocalMousePosition();
    
    FootKungFu? FootKungFu { get; }
    
    KungFu? AttackKungFu { get; }
    
    KungFu? ProtectionKungFu{ get; }
    
    KungFu? AssistantKungFu { get; }
    
    Connection Connection { get; }
    ValueBar LifeBar { get; }
    ValueBar PowerBar { get; }
    ValueBar InnerPowerBar { get; }
    ValueBar OuterPowerBar { get; }
    ValueBar HeadLifeBar { get; }
    ValueBar ArmLifeBar { get; }
    ValueBar LegLifeBar { get; }

}