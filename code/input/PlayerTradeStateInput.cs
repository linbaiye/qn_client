using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class PlayerTradeStateInput : I2ServerMessage
{

    private readonly int _state;

    private PlayerTradeStateInput(int state)
    {
        this._state = state;
    }

    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            TradeStateInput = new PlayerTradeStateInputPacket()
            {
                State = _state
            }
        };
    }

    public static readonly PlayerTradeStateInput Cancel = new(1);
    public static readonly PlayerTradeStateInput Confirm = new(2);
    public static readonly PlayerTradeStateInput Unconfirmed = new(3);
}