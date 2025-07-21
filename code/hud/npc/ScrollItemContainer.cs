using System;
using System.Collections.Generic;
using Godot;
using QnClient.code.message;
using QnClient.code.sprite;

namespace QnClient.code.hud.npc;

public partial class ScrollItemContainer : ScrollContainer
{
    private VBoxContainer _itemContainer;
    private Texture2D[] _itemIconTexture2Ds;
    public event Action<Item>? ItemDoubleClicked;
    public override void _Ready()
    {
        _itemContainer = GetNode<VBoxContainer>("ItemContainer");
        _itemIconTexture2Ds = ZipFileSpriteLoader.Instance.LoadOrderedItemIcons();
    }

    private void OnItemClicked(Item item)
    {
        foreach (var child in _itemContainer.GetChildren())
        {
            if (child is Item i)
            {
                i.ToggleHighlight(false);
            }
        }
        item.ToggleHighlight(true);
    }

    private void OnItemDoubleClicked(Item item)
    {
        OnItemClicked(item);
        ItemDoubleClicked?.Invoke(item);
    }

    public void ShowItems(List<NpcSellMenuMessage.NpcItemMessage> items)
    {
        foreach (var child in _itemContainer.GetChildren())
        {
            _itemContainer.RemoveChild(child);
        }
        foreach (var msg in items)
        {
            Item item = Item.Create();
            _itemContainer.AddChild(item);
            item.Clicked += OnItemClicked;
            item.DoubleClicked += OnItemDoubleClicked;
            item.SetDetails(msg.Name, _itemIconTexture2Ds[msg.Icon], msg.Color, msg.Price, msg.CanStack, msg.Icon);
        }
    }
}