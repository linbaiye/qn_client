using QnClient.code.hud;
using QnClient.code.player;
using QnClient.code.player.character;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class PlayerEquipMessage : IPlayerMessage, ICharacterMessage, IHUDMessage
{
    private PlayerEquipMessage(long id, int color, string name, EquipmentType type, WeaponType weaponType, string spritePrefix, string pairedSpritePrefix)
    {
        Id = id;
        Color = color;
        Type = type;
        WeaponType = weaponType;
        SpritePrefix = spritePrefix;
        PairedSpritePrefix = pairedSpritePrefix;
        Name = name;
    }

    public string Name { get; }

    private bool BelongToCharacter { get; set; } = false;

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
        var paired = equipmentType == EquipmentType.Wrist ? packet.PairedPrefix : "";
        return new PlayerEquipMessage(packet.Id, packet.Color, packet.Name, equipmentType, weaponType, packet.Prefix, paired);
    }

    public void Accept(ICharacterMessageHandler handler)
    {
        BelongToCharacter = true;
        handler.Equip(this);
    }

    public void Accept(IHUDMessageHandler handler)
    {
        if (BelongToCharacter)
            handler.Equip(Type, SpritePrefix, Name, Color, PairedSpritePrefix);
    }
}