using Godot;
using QnClient.code.hud.inventory;
using QnClient.code.input;
using QnClient.code.message;
using QnClient.code.network;

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
        Visible = false;
    }

    public void SetConnection(Connection connection)
    {
        _connection = connection;
    }

    public void SetInputInventory(ItemModifyInput input, Inventory inventory)
    {
        _input = input;
        _inventory = inventory;
    }

    private void OnCancel()
    {
        _inventory.DoubleClickedHandler = null;
        _connection.WriteAndFlush(PlayerTradeStateInput.Cancel);
    }

    private void OnConfirmPressed()
    {
        if (_confirm.IsPressed())
        {
            _connection.WriteAndFlush(PlayerTradeStateInput.Confirm);
        }
        else
        {
            _connection.WriteAndFlush(PlayerTradeStateInput.Unconfirmed);
        }
    }

    private void ClearBox(HBoxContainer container)
    {
        foreach (var child in container.GetChildren())
        {
            if (child is Slot slot)
            {
                slot.Clear();
            }
        }
    }

    private void OnInventorySlotDoubleClicked(int slot, string itemName, long number)
    {
        
    }

    private void OnInputConfirmed(ItemModifyInput input)
    {
        var slot = _input.GetExtra<int>("slot");
        _connection.WriteAndFlush(new AddPlayerTradeItemInput(slot, input.Number));
        _input.SetInUse(false);
    }

    public void Close()
    {
        _inventory.DoubleClickedHandler = null;
        Visible = false;
        _input.SetInUse(false);
    }

    public void OpenWindow(OpenPlayerTradeWindowMessage message)
    {
        ClearBox(_selfBox);
        ClearBox(_anotherBox);
        _inventory.DoubleClickedHandler = OnInventorySlotDoubleClicked;
        _selfName.Text = message.SelfName;
        _anotherName.Text = message.AnotherName;
        _input.Confirmed = OnInputConfirmed;
        if (message.Proactive)
        {
            _input.SetInUse(true);
            _input.SetExtra("max", message.MaxNumber);
            _input.SetExtra("slot", message.Slot);
            _input.SetNameNumber(message.ItemName, 1);
        }
        Visible = true;
    }
}