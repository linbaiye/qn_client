using System;
using Godot;
using QnClient.code.hud.inventory;
using QnClient.code.input;
using QnClient.code.message;

namespace QnClient.code.hud.npc;

public partial class NpcTradeMenu : AbstractNpcMenu
{
    private ScrollItemContainer _itemContainer;


    private ItemModifyInput _input;
    
    public override void _Ready()
    {
        base._Ready();
        _itemContainer = GetNode<ScrollItemContainer>("ScrollItemContainer");
        _itemContainer.ItemDoubleClicked += OnItemDoubleClicked;
        GetNode<Button>("Cancel").Pressed += () => Visible = false;
    }

    public void SetInput(ItemModifyInput input)
    {
        _input = input;
        _input.Confirmed = ConfirmBuy;
    }

    private void OnItemDoubleClicked(Item item)
    {
        _input.SetInUse(true);
        if (item.CanStack)
        {
            _input.SetNameNumberFocus(item.ItemName,0);
        }
        else
        {
            _input.SetNameNumber(item.ItemName,1);
            _input.ToggleEditable(false);
        }
        _input.SetExtra("item", item);
    }

    private void ConfirmBuy(ItemModifyInput input)
    {
        var item = input.GetExtra<Item>("item");
        SendMessage(new BuyItemInput(NpcId, item.ItemName, input.Number));
        _input.SetInUse(false);
    }

    public void ShowSellMenu(NpcSellMenuMessage message)
    {
        SetFields(message.Name, message.Id, message.Greetings, message.Sprite, message.Image);
        _itemContainer.ShowItems(message.Items);
        Visible = true;
    }
}