using System.Collections.Generic;
using Godot;
using QnClient.code.hud;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class InteractablePositionNameMessage(IEnumerable<InteractablePositionNameMessage.PositionAndName> npcPositions) : IHUDMessage
{
    public class PositionAndName(Vector2I coordinate, string name)
    {
        public Vector2I Coordinate { get; } = coordinate;
        public string Name { get; } = name;
    }
    
    public IEnumerable<PositionAndName> NpcPositions { get; } = npcPositions;


    public static InteractablePositionNameMessage FromPacket(NpcPositionPacket packet)
    {
        List<PositionAndName> npcPositions = new List<PositionAndName>();
        for (int i = 0; i < packet.XList.Count; i++)
        {
            var x = packet.XList[i];
            var y = packet.YList[i];
            var name  = packet.NameList[i];
            npcPositions.Add(new PositionAndName(new Vector2I(x, y), name));
        }
        return new InteractablePositionNameMessage(npcPositions);
    }

    public void Accept(IHUDMessageHandler handler)
    {
        handler.ShowCoordinateNameOnMap(this);
    }
}