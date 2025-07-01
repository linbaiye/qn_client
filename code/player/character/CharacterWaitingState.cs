
namespace QnClient.code.player.character;

/**
 * The state that responds no input, waiting for the further state change message.
 */
public class CharacterWaitingState : AbstractCharacterState
{
    public static readonly CharacterWaitingState Instance = new CharacterWaitingState();
}