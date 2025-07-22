using System.Collections.Generic;
using QnClient.code.hud;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class NpcTradeMenuMessage(
    long id,
    string name,
    string sprite,
    List<NpcTradeMenuMessage.NpcItemMessage> items,
    int image,
    string greetings,
    bool sale)
    : IHUDMessage
{
    public long Id { get;  } = id;
    public string Name { get;  } = name;
    public string Sprite { get;  } = sprite;
    public List<NpcItemMessage> Items = items;
    public int Image = image;
    public string Greetings { get;  } = greetings;

    public bool Sale { get; } = sale;

    public readonly struct NpcItemMessage(string name, int price, int icon, int color, bool canStack)
    {
        public string Name { get; } = name;
        public int Price { get; } = price;
        public int Icon { get; } = icon;
        public int Color { get; } = color;
        public bool CanStack { get; } = canStack;
    }
    
    public void Accept(IHUDMessageHandler handler)
    {
        handler.ShowNpcSellMenu(this);
    }

    public static NpcTradeMenuMessage FromPacket(NpcTradeMenuPacket packet)
    {
        List<NpcItemMessage> result = new List<NpcItemMessage>();
        foreach (var item in packet.Items)
        {
            NpcItemMessage message = new NpcItemMessage(item.Name, item.Price, item.Icon, item.Color, item.CanStack);
            result.Add(message);
        }
        return new NpcTradeMenuMessage(packet.Id, packet.Name, packet.Sprite, result, packet.Image, packet.Greetings, packet.Sale);
    }
}