using QnClient.code.entity;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class DynamicObjectShiftMessage(long id, int aniId, int ani2, bool lift) : IDynamicObjectMessage
{
    public long Id { get; } = id;

    private bool LiftCoordinates { get; } = lift;
    
    public void Accept(IDynamicObjectMessageHandler handler)
    {
        handler.Shift(aniId, ani2, LiftCoordinates);
    }

    public static DynamicObjectShiftMessage FromPacket(DynamicObjectShiftPacket packet)
    {
        return new DynamicObjectShiftMessage(packet.Id, packet.AnimationId, packet.AnimationId2,
            packet.LiftCoordinates);
    }
}