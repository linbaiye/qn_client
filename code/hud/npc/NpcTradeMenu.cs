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

    private Inventory _inventory;
    
    private Action<string> _errorHandler;
    
    public override void _Ready()
    {
        base._Ready();
        _itemContainer = GetNode<ScrollItemContainer>("ScrollItemContainer");
        _itemContainer.ItemDoubleClicked += OnItemDoubleClicked;
        GetNode<Button>("Cancel").Pressed += OnClose;
    }

    protected override void OnClose()
    {
        _inventory.DoubleClickedHandler = null;
        Visible = false;
    }

    public void SetInputInventory(ItemModifyInput input, Inventory inventory, Action<string> errorHandler)
    {
        _input = input;
        _inventory = inventory;
        _errorHandler = errorHandler;
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

    private void ConfirmPlayerBuy(ItemModifyInput input)
    {
        var item = input.GetExtra<Item>("item");
        SendMessage(new BuyItemInput(NpcId, item.ItemName, input.Number));
        _input.SetInUse(false);
    }


    private void OnInventoryItemDoubleClicked(int slot, string name, long maxNumber)
    {
        foreach (var item in _itemContainer.GetItems)
        {
            if (item.ItemName.Equals(name))
            {
                _input.SetInUse(true);
                _input.SetExtra("slot", slot);
                _input.SetExtra("maxNumber", maxNumber);
                _input.SetNameNumberFocus(name, (int)maxNumber);
                return;
            }
        }
        _errorHandler.Invoke("不买此种物品。");
    }

    private void ConfirmPlayerSell(ItemModifyInput input)
    {
        var max = input.GetExtra<long>("maxNumber");
        if (max < input.Number)
        {
            _errorHandler.Invoke("数量不足。");
            return;
        }
        SendMessage(new SellItemInput(NpcId, input.GetExtra<int>("slot"), input.Number));
        input.SetInUse(false);
    }

    public void ShowSellMenu(NpcTradeMenuMessage message)
    {
        SetFields(message.Name, message.Id, message.Greetings, message.Sprite, message.Image);
        if (message.Sale)
        {
            _itemContainer.ShowSellItems(message.Items);
            _input.Confirmed = ConfirmPlayerBuy;
        }
        else
        {
            _itemContainer.ShowBuyItems(message.Items);
            _input.Confirmed = ConfirmPlayerSell;
            _inventory.DoubleClickedHandler = OnInventoryItemDoubleClicked;
        }
        Visible = true;
    }
}