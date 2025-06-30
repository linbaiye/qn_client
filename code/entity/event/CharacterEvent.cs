using QnClient.code.player.character;

namespace QnClient.code.entity.@event;

public class CharacterEvent(ICharacter character, CharacterEvent.EventType type) : IEntityEvent
{
    public IEntity Source { get; } = character;
    
    public ICharacter Character  => (ICharacter) Source ;

    public EventType Type { get; } = type;
    
    public enum EventType
    {
        Joined,
        SyncActiveKungFu,
    }

    public static CharacterEvent Join(ICharacter character)
    {
        return new CharacterEvent(character, EventType.Joined);
    }
}