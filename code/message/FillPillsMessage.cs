using System.Collections.Generic;
using QnClient.code.hud;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class FillPillsMessage(List<string> pills) : IHUDMessage
{
    public void Accept(IHUDMessageHandler handler)
    {
        handler.FillPills(pills);
    }

    public static FillPillsMessage FromPacket(PillsPacket packet)
    {
        List<string> pills = new List<string>();
        foreach (var packetPill in packet.Pills)
        {
            pills.Add(packetPill);
        }
        return new FillPillsMessage(pills);
    }
}