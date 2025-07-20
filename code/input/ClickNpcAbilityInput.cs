using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class ClickNpcAbilityInput(long id, string abilityName) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            ClickNpcAbilityInput = new ClickNpcAbilityInputPacket()
            {
                Id = id,
                AbilityName = abilityName
            }
        };

    }
}