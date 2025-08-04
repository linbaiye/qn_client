using System;
using System.Collections.Generic;
using Godot;
using QnClient.code.hud.bottom;
using QnClient.code.message;
using QnClient.code.player;
using QnClient.code.sprite;

namespace QnClient.code.hud.attribute;

public partial class Equipment : NinePatchRect
{
    private VBoxContainer _left;
    
    private VBoxContainer _right;

    private Texture2D[] _itemIcons;
    
    private Slot[] _slots = new Slot[8];
    
    private EquipView _equipView;
    public event Action<EquipmentType>? UnequipPressed;
    public event Action<EquipmentType>? RightPressed;

    private static readonly Dictionary<EquipmentType, int> IndexMap = new()
    {
        { EquipmentType.Hair, 0 },
        { EquipmentType.Armor, 1 },
        { EquipmentType.Weapon, 2 },
        { EquipmentType.Leg, 3 },
        { EquipmentType.Hat, 4 },
        { EquipmentType.Vest, 5 },
        { EquipmentType.Wrist, 6 },
        { EquipmentType.Boot, 7 },
    };
    public override void _Ready()
    {
        _itemIcons = ZipFileSpriteLoader.Instance.LoadOrderedItemIcons();
        _left = GetNode<VBoxContainer>("Left");
        _right = GetNode<VBoxContainer>("Right");
        for (int i = 1; i <= 8; i++)
        {
            var slot = Slot.Create("Slot" + i, new Vector2(34, 34), new Vector2(34, 34), false);
            if (i <= 4)
                _left.AddChild(slot);
            else
                _right.AddChild(slot);
            _slots[i - 1] = slot;
            _slots[i - 1].RightMouseButtonReleased += n => HandleEvent(n, false);
            _slots[i - 1].LeftMouseButtonDoubleClicked += n => HandleEvent(n, true);
        }
        _equipView = GetNode<EquipView>("EquipView");
    }


    private void HandleEvent(int number, bool unequip)
    {
        if (_slots[number - 1].Empty)
            return;
        foreach (var keyValuePair in IndexMap)
        {
            if (keyValuePair.Value == number - 1)
            {
                if (unequip)
                    UnequipPressed?.Invoke(keyValuePair.Key);
                else
                    RightPressed?.Invoke(keyValuePair.Key);
                break;
            }
        }
    }


    public void Equip(PlayerEquipMessage message)
    {
        _equipView.Equip(message);
        if (IndexMap.TryGetValue(message.Type, out var index))
        {
            _slots[index].ClearTextureAndTip();
            _slots[index].SetDetails(_itemIcons[message.Icon], message.Name, message.Color);
        }
    }

    public void Unequip(EquipmentType type)
    {
        _equipView.Unequip(type);
        if (IndexMap.TryGetValue(type, out var index))
        {
            _slots[index].ClearTextureAndTip();
        }
    }

    public void ShowEquipments(bool male, List<PlayerEquipMessage> equipments)
    {
        foreach (var slot in _slots)
        {
            slot.ClearTextureAndTip();
        }
        foreach (var message in equipments)
        {
            if (IndexMap.TryGetValue(message.Type, out var index))
            {
                _slots[index].SetDetails(_itemIcons[message.Icon], message.Name, message.Color);
            }
        }
        _equipView.Display(male, equipments);
    }

    public void ShowEquipmentDescription(EquipmentType type, string text)
    {
        if (IndexMap.TryGetValue(type, out var idx))
        {
            _slots[idx].ShowAttributeTipIfHasHover(text);
        }
    }
}