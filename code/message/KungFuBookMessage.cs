
using System.Collections.Generic;
using QnClient.code.ui;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class KungFuBookMessage(List<KungFuBookMessage.KungFu> unnamed, List<KungFuBookMessage.KungFu> basic) : IHUDMessage
{

    public List<KungFu> Unnamed { get; } = unnamed;
    public List<KungFu> Basic { get; } = basic;
    
    public readonly struct KungFu(int l, string name, int icon, int slot)
    {
        private string Name { get; } = name;
        private string Level { get; } = l / 100 + "." + (l % 100).ToString("00");
        public int Icon { get; } = icon;
        public int Slot { get; } = slot;

        public string ToolTip => Name + ": " + Level;
    }


    public static KungFuBookMessage FromPacket(KungFuBookPacket packet)
    {
        List<KungFu> unnamed = new List<KungFu>();
        foreach (var kungFuPacket in packet.UnnamedKungFuList)
        {
            unnamed.Add(new KungFu(kungFuPacket.Level, kungFuPacket.Name, kungFuPacket.Icon, kungFuPacket.Slot));
        }
        List<KungFu> basic = new List<KungFu>();
        foreach (var kungFuPacket in packet.BasicKungFuList)
        {
            basic.Add(new KungFu(kungFuPacket.Level, kungFuPacket.Name, kungFuPacket.Icon, kungFuPacket.Slot));
        }
        return new KungFuBookMessage(unnamed, basic);
    }

    public void Accept(IHUDMessageHandler handler)
    {
        handler.UpdateKungFuBookView(this);
    }
}