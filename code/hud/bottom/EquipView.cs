using Godot;
using QnClient.code.message;
using QnClient.code.player;
using QnClient.code.sprite;

namespace QnClient.code.hud.bottom;

public partial class EquipView : NinePatchRect
{
    private bool _male;
    
    private static readonly ZipFileSpriteLoader SpriteLoader = ZipFileSpriteLoader.Instance;
    
    private const int AvatarIndex = 57;

    private TextureRect _body;
    private TextureRect _weapon;

    private Vector2 _bodyOffset;


    public override void _Ready()
    {
        _body = GetNode<TextureRect>("Body");
        _weapon = GetNode<TextureRect>("Weapon");
    }
    

    public void OnCharacterJoined(JoinRealmMessage message)
    {
        _male = message.Male;
        var textures = SpriteLoader.Load(_male ? "N00" : "A00");
        var offsetTexture = textures[AvatarIndex];
        _body.Texture = offsetTexture.Texture;
        _body.Position = new Vector2((Size.X - offsetTexture.OriginalSize.X) / 2, (Size.Y - offsetTexture.OriginalSize.Y) / 2);
        _bodyOffset = _body.Position - offsetTexture.Offset;
        foreach (var equip in message.Equipments)
        {
            if (equip.Type == EquipmentType.Weapon)
            {
                textures = SpriteLoader.Load(equip.SpritePrefix  + "0");
                offsetTexture = textures[AvatarIndex];
                _weapon.Texture = offsetTexture.Texture;
                _weapon.Position = _bodyOffset + offsetTexture.Offset;
            }
        }
    }
}