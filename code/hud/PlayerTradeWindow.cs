using Godot;
using QnClient.code.hud.inventory;
using QnClient.code.input;
using QnClient.code.message;
using QnClient.code.network;
using QnClient.code.sprite;

namespace QnClient.code.hud;

public partial class PlayerTradeWindow : NinePatchRect, IConnectionAware
{

    private Label _selfName;
    
    private Label _anotherName;

    private HBoxContainer _selfBox;
    
    private HBoxContainer _anotherBox;

    private Connection _connection;

    private ItemModifyInput _input;
    
    private Inventory _inventory;

    private TextureButton _confirm;
    
    private Button _cancel;
    
    public override void _Ready()
    {
        _selfName = GetNode<Label>("SelfName");
        _anotherName = GetNode<Label>("AnotherName");
        _selfBox = GetNode<HBoxContainer>("SelfBox");
        _anotherBox = GetNode<HBoxContainer>("AnotherBox");
        _confirm = GetNode<TextureButton>("Confirm");
        _cancel = GetNode<Button>("Cancel");
        _cancel.Pressed += OnCancel;
        _confirm.Pressed += OnConfirmPressed;
        AddSlots(_selfBox);
        AddSlots(_anotherBox);
        _input = ItemModifyInput.Create();
        var p = new Vector2(GetSize().X, GetSize().Y - _input.GetSize().Y);
        _input.Position = p;
        AddChild(_input);
        _input.Confirmed = OnInputConfirmed;
        Visible = false;
    }


    private void AddSlots(HBoxContainer container)
    {
        for (int slot = 1; slot <= 4; slot++)
        {
            var s = Slot.Create("Slot" + slot, new Vector2(40, 40), new Vector2(30, 30), false);
            container.AddChild(s);
        }
    }

    public void SetConnection(Connection connection)
    {
        _connection = connection;
    }

    public void SetInventory(Inventory inventory)
    {
        _inventory = inventory;
    }

    private void OnCancel()
    {
        _connection.WriteAndFlush(PlayerTradeStateInput.Cancel);
    }

    private void OnConfirmPressed()
    {
        _connection.WriteAndFlush(_confirm.IsPressed()
            ? PlayerTradeStateInput.Confirm
            : PlayerTradeStateInput.Unconfirmed);
    }

    private void ClearBox(HBoxContainer container)
    {
        foreach (var child in container.GetChildren())
        {
            if (child is Slot slot)
            {
                slot.ClearTextureAndTip();
            }
        }
    }

    private void OnInventorySlotDoubleClicked(int slot, string itemName, long number)
    {
        _input.SetInUse(true);
        _input.SetExtra("slot", slot);
        _input.SetNameNumber(itemName, number);
    }

    private void OnInputConfirmed(ItemModifyInput input)
    {
        var slot = _input.GetExtra<int>("slot");
        _connection.WriteAndFlush(new AddPlayerTradeItemInput(slot, input.Number));
        _input.SetInUse(false);
    }

    public void Close()
    {
        _inventory.UninstallDoubleClickHandler(this);
        Visible = false;
        _input.SetInUse(false);
    }

    public void OpenWindow(OpenPlayerTradeWindowMessage message)
    {
        ClearBox(_selfBox);
        ClearBox(_anotherBox);
        _inventory.InstallDoubleClickHandler(this, OnInventorySlotDoubleClicked);
        _selfName.Text = message.SelfName;
        _anotherName.Text = message.AnotherName;
        if (message.Proactive)
        {
            _input.SetInUse(true);
            _input.SetExtra("max", message.MaxNumber);
            _input.SetExtra("slot", message.Slot);
            _input.SetNameNumber(message.ItemName, 1);
        }
        _confirm.SetPressed(false);
        Visible = true;
    }

    private void UpdateSlot(HBoxContainer container, InventoryItemMessage message)
    {
        var slot = container.GetNode<Slot>("Slot" + (message.Slot));
        var icons = ZipFileSpriteLoader.Instance.LoadOrderedItemIcons();
        slot.ClearTextureAndTip();
        slot.SetDetails(icons[message.Icon], message.Tip, message.Color);
    }

    public void UpdateSlot(bool self, InventoryItemMessage message)
    {
        UpdateSlot(self ? _selfBox : _anotherBox, message);
    }
}