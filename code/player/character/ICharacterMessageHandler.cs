using Godot;
using QnClient.code.entity;
using QnClient.code.message;

namespace QnClient.code.player.character;

public interface ICharacterMessageHandler : IBasePlayerMessageHandler
{
    void SyncActiveKungFuList(SyncActiveKungFuListMessage message);
    
    void SetPosition(Vector2I coordinate, PlayerState state, CreatureDirection direction);
    
}