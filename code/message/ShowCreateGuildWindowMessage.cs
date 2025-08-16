using QnClient.code.hud;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class ShowCreateGuildWindowMessage(int slotId, string title, string tip) : IHUDMessage
{
    private int SlotId { get; } = slotId;
    private string Title { get; } = title;
    private string Tip { get; } = tip;


    public void Accept(IHUDMessageHandler handler)
    {
        handler.ShowCreateGuildWindow(SlotId, Title, Tip);
    }

    public static ShowCreateGuildWindowMessage FromPacket(ShowCreateGuildWindowPacket packet)
    {
        return new ShowCreateGuildWindowMessage(packet.FromSlot, packet.Tile, packet.Tip);
    }
}