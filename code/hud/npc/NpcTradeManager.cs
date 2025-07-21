
using System;
using System.Collections.Generic;
using QnClient.code.hud.inventory;
using QnClient.code.message;

namespace QnClient.code.hud.npc;

public class NpcTradeManager(
    NpcTradeMenu npcTradeMenu,
    NpcMainMenu mainMenu,
    ItemModifyInput input,
    Inventory inventory,
    Action<string> errorHandler)
{
    private ITrade? _currentTrade;
    public void ShowNpcMenu(NpcMenuMessage message)
    {
        if (input.Using)
        {
            errorHandler.Invoke("另一操作正在进行中。");
            return;
        }
        _currentTrade?.Rollback();
        npcTradeMenu.Visible = false;
        mainMenu.Show(message);
    }

    public void ShowNpcSellMenu(NpcSellMenuMessage message)
    {
        if (input.Using)
        {
            errorHandler.Invoke("另一操作正在进行中。");
            return;
        }
        mainMenu.Visible = false;
        npcTradeMenu.ShowSellMenu(message);
        _currentTrade = new PlayerBuyTrade(inventory, npcTradeMenu, input, errorHandler);
    }
    
    private interface ITrade
    {
        void Rollback();
    }

    private class PlayerBuyTrade : ITrade
    {
        private readonly Inventory _inventory;
        private readonly NpcTradeMenu _tradeMenu;
        private readonly ItemModifyInput _input;
        private readonly List<InventoryItemMessage> _items;
        private readonly Action<string> _errorHandler;
        public PlayerBuyTrade(Inventory inventory, NpcTradeMenu tradeMenu, ItemModifyInput input, Action<string> errorHandler)
        {
            _inventory = inventory;
            _tradeMenu = tradeMenu;
            _input = input;
            _input.Confirmed = OnInputConfirm;
            _items = _inventory.CloneItems();
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

        private void OnTradeCancel()
        {
            
        }
        
        private void OnTradeConfirm()
        {
            
        }

        private void OnInputConfirm(ItemModifyInput input)
        {
            var item = input.GetExtra<Item>("item");
            int cost = input.Number * item.Price;
            if (!_inventory.CanAfford(cost))
            {
                _errorHandler.Invoke("钱币不足。");
            }
            else
            {
                item.AddCost(input.Number * item.Price);
                if (!item.CanStack)
                    _inventory.AddNonStack(item.ItemName, item.Icon, item.IconColor);
            }
            input.SetInUse(false);
        }
        
        public void Rollback()
        {
            _inventory.Update(_items);
        }
    }
}