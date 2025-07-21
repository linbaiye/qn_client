using System;
using Godot;
using QnClient.code.hud.inventory;
using QnClient.code.message;

namespace QnClient.code.hud.npc;

public partial class NpcTradeMenu : AbstractNpcMenu
{
    private ScrollItemContainer _itemContainer;


    public Action<Item>? ItemDoubleClicked;
    
    public override void _Ready()
    {
        base._Ready();
        _itemContainer = GetNode<ScrollItemContainer>("ScrollItemContainer");
        _itemContainer.ItemDoubleClicked += i => ItemDoubleClicked?.Invoke(i);
        GetNode<Button>("Cancel").Pressed += () => Visible = false;
    }

    private void OnItemDoubleClicked(Item item)
    {

    }

    private void ConfirmBuy(ItemModifyInput input)
    {
    }

    public void ShowSellMenu(NpcSellMenuMessage message)
    {
        SetFields(message.Name, message.Id, message.Greetings, message.Sprite, message.Image);
        _itemContainer.ShowItems(message.Items);
        Visible = true;
    }
}