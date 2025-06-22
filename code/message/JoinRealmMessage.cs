using Godot;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class JoinRealmMessage : IMessage
{
    public string MapFile { get; private init; }
    public string ResourceName { get; private init; }
    
    public string Name { get; set; }
    
    public Vector2I Coordinate { get; private init; }
    
    public long Id { get; private init; }

    public static JoinRealmMessage Parse(LoginPacket loginPacket)
    {
        var map = loginPacket.Teleport.Map.Replace(".map", "");
        var resource = loginPacket.Teleport.Resource.Replace(".zip", "");
        return new JoinRealmMessage()
        {
            MapFile = map,
            ResourceName = resource,
            Coordinate = new Vector2I(loginPacket.Teleport.X, loginPacket.Teleport.Y),
            Name = loginPacket.Info.Name,
            Id = loginPacket.Info.Id,
        };
    }

    public void Accept(IMessageHandler messageHandler)
    {
        messageHandler.Handle(this);
    }
}