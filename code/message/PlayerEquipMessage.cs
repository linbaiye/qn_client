using QnClient.code.player;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class PlayerEquipMessage : IPlayerMessage
{
    private PlayerEquipMessage(long id, int color, EquipmentType type, WeaponType weaponType, string spritePrefix, string pairedSpritePrefix)
    {
        Id = id;
        Color = color;
        Type = type;
        WeaponType = weaponType;
        SpritePrefix = spritePrefix;
        PairedSpritePrefix = pairedSpritePrefix;
    }

    public long Id { get; }
    
    public int Color { get; }
    
    public EquipmentType Type { get; }
    
    public WeaponType WeaponType { get; }
    
    public string SpritePrefix { get; }
    
    public string PairedSpritePrefix { get; }
    
    public void Accept(IPlayerMessageHandler handler)
    {
        handler.Equip(this);
    }

    public static PlayerEquipMessage FromPacket(PlayerEquipPacket packet)
    {
        EquipmentType equipmentType = (EquipmentType)packet.EquipmentType;
        WeaponType weaponType = equipmentType == EquipmentType.Weapon ? (WeaponType)packet.WeaponType : WeaponType.None;
        var paired = equipmentType == EquipmentType.Wrist ? packet.PairedSpritePrefix : "";
        return new PlayerEquipMessage(packet.Id, packet.Color, equipmentType, weaponType, packet.SpritePrefix, paired);
    }
}