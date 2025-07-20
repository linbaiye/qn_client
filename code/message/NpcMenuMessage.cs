using System.Collections.Generic;
using QnClient.code.hud;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class NpcMenuMessage(
    long id,
    string name,
    string greetings,
    string sprite,
    int image,
    List<string> supportedActions)
    : IHUDMessage
{
    public long Id { get; } = id;
    public string Name { get; } = name;

    public string Greetings {get;} = greetings;

    public string Sprite { get; } = sprite;

    public int Image { get; } = image;
    

    public readonly List<string> SupportedActions = supportedActions;

    public static NpcMenuMessage FromPacket(NpcMenuPacket packet)
    {
        List<string> actions = new List<string>();
        foreach (var packetSupportedAction in packet.SupportedActions)
        {
            actions.Add(packetSupportedAction);
        }
        return new NpcMenuMessage(packet.Id, packet.Name, packet.Greetings, packet.Sprite, packet.Image, actions);
    }

    public void Accept(IHUDMessageHandler handler)
    {
        handler.ShowNpcMenu(this);
    }
}