using System.Collections.Generic;
using System.Linq;
using QnClient.code.player;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class PlayerSnapshot : AbstractCreatureSnapshot, IPlayerMessage, ISpriteMessage
{
    private PlayerSnapshot(PlayerSnapshotPacket packet, List<PlayerEquipMessage> equipMessages)  : base(packet.BaseInfo)
    {
        Male = packet.Male;
        PlayerState = (PlayerState)packet.State;
        MoveAction = packet.MoveAction != -1 ? (MoveAction)packet.MoveAction : MoveAction.None;
        EquipMessages = equipMessages;
    }
    public bool Male { get; private init; }
    
    public PlayerState PlayerState { get; private init; }
    
    public MoveAction MoveAction { get; private init; }
    
    public List<PlayerEquipMessage> EquipMessages { get; private init; }

    public static PlayerSnapshot FromPacket(PlayerSnapshotPacket packet)
    {
        List<PlayerEquipMessage> equips = new List<PlayerEquipMessage>();
        foreach (var playerEquipPacket in packet.Equipments)
        {
            equips.Add(PlayerEquipMessage.FromPacket(playerEquipPacket));
        }
        return new PlayerSnapshot(packet, equips);
    }

    public void Accept(IPlayerMessageHandler handler)
    {
        handler.Initialize(this);
    }

    public string[] Sprites
    {
        get
        {
            List<string> sprites = new List<string>();
            string prefix = Male ? "N0" : "A0";
            for (int i = 0; i < 5; i++)
            {
                sprites.Add(prefix + i);
            }
            return sprites.ToArray();
        }
    }
}