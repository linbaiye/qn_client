using Godot;
using Microsoft.VisualBasic.CompilerServices;
using QnClient.code.hud.inventory;
using QnClient.code.input;
using QnClient.code.message;
using QnClient.code.network;
using QnClient.code.sprite;

namespace QnClient.code.hud.bank;

public partial class Bank : AbstractSlotContainer, IConnectionAware
{

    private ItemModifyInput _input;
    
    private Connection _connection;

    private ShowBankMessage _message;

    private Texture2D[] _icons;

    private Button _unlock;

    private long _npcId;

    private Inventory _inventory;

    private Texture2D _lock;

    public override void _Ready()
    {
        base._Ready();
        _input = ItemModifyInput.Create();
        var p = new Vector2(GetSize().X, GetSize().Y - _input.GetSize().Y);
        _input.Position = p;
        AddChild(_input);
        _icons = ZipFileSpriteLoader.Instance.LoadOrderedItemIcons();
        _unlock = GetNode<Button>("Unlock");
        _unlock.Pressed += () => _connection?.WriteAndFlush(new UnlockBankInput(_npcId));
        _lock =  ResourceLoader.Load<Texture2D>("res://ui/bank/lock.png");
        Visible = false;
    }

    protected override Slot CreateSlot(string name)
    {
        return Slot.Create(name, new Vector2(40, 40), new Vector2(30, 30), false);
    }
    
    private string GetItemName(int slot)
    {
        foreach (var item in _message.ItemMessages)
        {
            if (item.Slot == slot)
                return item.Name;
        }
        return null;
    }

    private string GetItemName(Slot slot)
    {
        var strings = slot.TooltipText.Split(":");
        return strings[0];
    }
    
    private int GetItemNumber(Slot slot)
    {
        var strings = slot.TooltipText.Split(":");
        return strings.Length > 1? int.Parse(strings[1]) : 1;
    }

    protected override void OnNonEmptyDragReleased(int number)
    {
        var hoveringSlot = FindSlotHasHovering();
        if (hoveringSlot != null)
        {
            _connection?.WriteAndFlush(BankOperationInput.Swap(_npcId, number, hoveringSlot.SlotNumber));
            return;
        }
        var invSlot = _inventory.FindSlotHasHovering();
        if (invSlot == null)
            return;
        var bankSlot = GetSlot(number);
        var itemNumber = GetItemNumber(bankSlot);
        if (itemNumber == 1)
        {
            _connection?.WriteAndFlush(BankOperationInput.BankToInventory(_npcId, number, invSlot.SlotNumber, 1));
            return;
        }
        _input.SetInUse(true);
        _input.SetExtra("bankSlot", number);
        _input.SetExtra("inventorySlot", invSlot.SlotNumber);
        _input.SetNameNumberFocus(GetItemName(bankSlot), itemNumber);
        _input.Confirmed = ConfirmBankToInventory;
    }

    private void ConfirmBankToInventory(ItemModifyInput input)
    {
        var bankSlot = input.GetExtra<int>("bankSlot");
        var inventorySlot = input.GetExtra<int>("inventorySlot");
        _connection?.WriteAndFlush(BankOperationInput.BankToInventory(_npcId, bankSlot, inventorySlot, _input.Number));
        _input.SetInUse(false);
    }

    protected override void OnSlotLeftButtonDoubleClicked(int number)
    {
        var bankSlot = GetSlot(number);
        var itemNumber = GetItemNumber(bankSlot);
        if (itemNumber == 1)
        {
            _connection?.WriteAndFlush(BankOperationInput.BankToInventory(_npcId, number, 1));
            return;
        }
        _input.SetInUse(true);
        _input.SetExtra("bankSlot", number);
        _input.SetNameNumberFocus(GetItemName(bankSlot), itemNumber);
        _input.Confirmed = ConfirmBankToInventoryEmpty;
    }
    
    private void ConfirmBankToInventoryEmpty(ItemModifyInput input)
    {
        var bankSlot = input.GetExtra<int>("bankSlot");
        _connection?.WriteAndFlush(BankOperationInput.BankToInventory(_npcId, bankSlot, _input.Number));
        _input.SetInUse(false);
    }

    protected override void OnSlotRightMouseButtonReleased(int number)
    {
        _connection?.WriteAndFlush(BankOperationInput.RightClick(_npcId, number));
    }

    private void ConfirmInventoryToBank(ItemModifyInput input)
    {
        var invSlot = _input.GetExtra<int>("inventorySlot");
        var bankSlot = _input.GetExtra<int>("bankSlot");
        _connection?.WriteAndFlush(BankOperationInput.InventoryToBank(_npcId, invSlot, bankSlot, _input.Number));
        _input.SetInUse(false);
    }

    private bool IsSlotUnlocked(int slotNumber)
    {
        return slotNumber >= 1 && slotNumber <= _message.Unlocked;
    }

    public void Show(ShowBankMessage message, Inventory inventory)
    {
        _message = message;
        _inventory = inventory;
        if (message.Capacity != Capacity)
            throw new IncompleteInitialization();
        ForeachSlot(s => s.ClearTextureAndTip());
        ForeachSlot(s =>
        {
            if (s.SlotNumber > message.Unlocked)
            {
                s.SetDetails(_lock, "");
            }
        });
        foreach (var item in message.ItemMessages)
        {
            GetSlot(item.Slot)
                .SetDetails(_icons[item.Icon], item.Tip, item.Color);
        }
        _npcId = message.NpcId;
        _inventory.InstallDoubleClickHandler(this, OnInventorySlotDoubleClicked);
        Visible = true;
    }

    public bool HandleInventoryDragItem(int inventorySlotNumber)
    {
        var slot = FindSlotHasHovering();
        if (slot == null)
            return false;
        if (!IsSlotUnlocked(slot.SlotNumber))
        {
            return true;
        }
        var inventorySlot = _inventory.GetSlot(inventorySlotNumber);
        if (inventorySlot == null)
            return true;
        var itemNumber = GetItemNumber(inventorySlot);
        if (itemNumber == 1)
        {
            _connection?.WriteAndFlush(BankOperationInput.InventoryToBank(_npcId, inventorySlotNumber, slot.SlotNumber, 1));
            return true;
        }
        _input.SetInUse(true);
        _input.SetExtra("inventorySlot", inventorySlotNumber);
        _input.SetExtra("bankSlot", slot.SlotNumber);
        _input.Confirmed = ConfirmInventoryToBank;
        _input.SetNameNumberFocus(GetItemName(inventorySlot), itemNumber);
        return true;
    }

    protected override void OnCloseButtonClicked()
    {
        Close();
    }

    public void Close()
    {
        _inventory?.UninstallDoubleClickHandler(this);
        _input.SetInUse(false);
        _message = null;
        Visible = false;
    }

    private void ConfirmInventoryToEmptyBankSlot(ItemModifyInput input)
    {
        var invSlot = _input.GetExtra<int>("inventorySlot");
        _connection?.WriteAndFlush(BankOperationInput.InventoryToBank(_npcId, invSlot, _input.Number));
        _input.SetInUse(false);
    }

    private void OnInventorySlotDoubleClicked(int slotNumber, string itemName, long itemNumber)
    {
        if (_message == null)
            return;
        if (itemNumber == 1)
        {
            _connection?.WriteAndFlush(BankOperationInput.InventoryToBank(_npcId, slotNumber, 1));
            return;
        }
        _input.SetInUse(true);
        _input.SetExtra("inventorySlot", slotNumber);
        _input.Confirmed = ConfirmInventoryToEmptyBankSlot;
        _input.SetNameNumberFocus(itemName, itemNumber);
    }

    protected override int Capacity => 40;
    public void SetConnection(Connection connection)
    {
        _connection = connection;
    }
}