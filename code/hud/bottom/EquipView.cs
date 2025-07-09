using System;
using System.Collections.Generic;
using Godot;
using NLog;
using QnClient.code.entity;
using QnClient.code.message;
using QnClient.code.player;
using QnClient.code.sprite;

namespace QnClient.code.hud.bottom;

public partial class EquipView : NinePatchRect
{
    private bool _male;
    
    private static readonly ZipFileSpriteLoader SpriteLoader = ZipFileSpriteLoader.Instance;
    
    private const int AvatarIndex = 57;

    private TextureButton _body;
    private TextureButton _leftWrist;
    private TextureButton _rightWrist;

    private Vector2 _bodyOffset;

    private Label _equipText;

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private Godot.Collections.Dictionary<EquipmentType, TextureButton> _equipments = new();
    private Godot.Collections.Dictionary<EquipmentType, string> _equipmentNames = new();


    public event Action<EquipmentType>? UnequipPressed;

    public override void _Ready()
    {
        _body = GetNode<TextureButton>("Body");
        _equipText = GetNode<Label>("EquipText");
        foreach(string name in Enum.GetNames<EquipmentType>())
        {
            var equipmentType = Enum.Parse<EquipmentType>(name);
            if (equipmentType == EquipmentType.Wrist)
                continue;
            var button = GetNode<TextureButton>(name);
            button.Pressed += () => OnPressed(equipmentType);
            button.MouseEntered += () => ShowName(equipmentType);
            button.MouseExited += () => _equipText.Text = string.Empty;
            _equipments.Add(equipmentType, button);
        }
        _leftWrist = GetNode<TextureButton>("LeftWrist");
        _leftWrist.MouseEntered += () => ShowName(EquipmentType.Wrist);
        _leftWrist.Pressed += () => OnPressed(EquipmentType.Wrist);
        _leftWrist.MouseExited += () => _equipText.Text = string.Empty;
        _rightWrist = GetNode<TextureButton>("RightWrist");
        _rightWrist.Pressed += () => OnPressed(EquipmentType.Wrist);
        _rightWrist.MouseEntered += () => ShowName(EquipmentType.Wrist);
        _rightWrist.MouseExited += () => _equipText.Text = string.Empty;
    }

    private void ShowName(EquipmentType type)
    {
        if (_equipmentNames.TryGetValue(type, out var name))
        {
            if (name.StartsWith("女子") || name.StartsWith("男子"))
                name = name.Substring(2, name.Length - 2);
            _equipText.Text = name;
        }
    }

    private void OnPressed(EquipmentType type)
    {
        UnequipPressed?.Invoke(type);
    }


    private void DyeIfHasColor(TextureButton button, int color)
    {
        var material = button.GetMaterial();
        if (material != null && color > 1)
        {
            DyeShader.SetColor((ShaderMaterial)material, color);
        }
    }

    public void Unequip(EquipmentType type)
    {
        if (type != EquipmentType.Wrist)
        {
            if (_equipments.TryGetValue(type, out var button))
            {
                button.TextureNormal = null;
                button.Size = new Vector2(1, 1);
            }
        }
        else
        {
            _leftWrist.TextureNormal = null;
            _rightWrist.TextureNormal = null;
            _leftWrist.Size = new Vector2(1, 1);
            _rightWrist.Size = new Vector2(1, 1);
        }
    }

    public void Equip(EquipmentType type, string prefix, string name, int color = 0, string pairedPrefix = null)
    {
        var textures = SpriteLoader.Load(prefix + "0");
        var offsetTexture = textures[AvatarIndex];
        _equipmentNames.Remove(type);
        _equipmentNames.TryAdd(type, name);
        if (type != EquipmentType.Wrist)
        {
            if (_equipments.TryGetValue(type, out var button))
            {
                button.TextureNormal = offsetTexture.Texture;
                button.Position = _bodyOffset + offsetTexture.Offset;
                DyeIfHasColor(button, color);
            }
        }
        else
        {
            _leftWrist.TextureNormal = offsetTexture.Texture;
            _leftWrist.Position = _bodyOffset + offsetTexture.Offset;
            DyeIfHasColor(_leftWrist, color);
            textures = SpriteLoader.Load(pairedPrefix + "0");
            offsetTexture = textures[AvatarIndex];
            _rightWrist.TextureNormal = offsetTexture.Texture;
            _rightWrist.Position = _bodyOffset + offsetTexture.Offset;
            DyeIfHasColor(_rightWrist, color);
        }
    }

    public void OnCharacterJoined(JoinRealmMessage message)
    {
        _male = message.Male;
        var textures = SpriteLoader.Load(_male ? "N00" : "A00");
        var offsetTexture = textures[AvatarIndex];
        _body.TextureNormal = offsetTexture.Texture;
        _body.Position = new Vector2((Size.X - offsetTexture.OriginalSize.X) / 2, (Size.Y - offsetTexture.OriginalSize.Y) / 2);
        _bodyOffset = _body.Position - offsetTexture.Offset;
        foreach (var equip in message.Equipments)
        {
            Equip(equip.Type, equip.SpritePrefix, equip.Name, equip.Color, equip.PairedSpritePrefix);
        }
    }
}