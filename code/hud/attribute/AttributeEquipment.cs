using Godot;
using QnClient.code.input;
using QnClient.code.message;
using QnClient.code.network;
using QnClient.code.player;

namespace QnClient.code.hud.attribute;

public partial class AttributeEquipment : NinePatchRect, IConnectionAware
{
    private Label[] _attributes = new Label[21];
    private Button _close;
    private Label _name;
    private Label _age;
    private Equipment _equipment;

    private Connection _connection;
    
    public override void _Ready()
    {
        for (int i = 0; i < _attributes.Length; i++)
        {
            _attributes[i] = GetNode<Label>("Label" + (i + 1));
        }
        Visible = false;
        _close = GetNode<Button>("Close");
        _close.Pressed += () => Visible = false;
        _name = GetNode<Label>("Name");
        _age = GetNode<Label>("Age");
        _equipment = GetNode<Equipment>("Equipment");
        _equipment.UnequipPressed += t => _connection?.WriteAndFlush(new UnequipInput(t));
        _equipment.RightPressed += t => _connection?.WriteAndFlush(new ClickEquipmentInput(t));
    }

    public void ShowAttributeEquipment(AttributeEquipmentMessage message)
    {
        if (message.Quietly && !Visible)
            return;
        for (int i = 0; i < message.Attributes.Length; i++)
        {
            _attributes[i].Text = message.Attributes[i];
        }
        _name.Text = message.Name;
        _age.Text = message.Age;
        if (!message.Quietly)
            _equipment.ShowEquipments(message.Male, message.Equipments);
        Visible = true;
    }
    
    public void OnAvatarPressed()
    {
        if (!Visible)
            _connection?.WriteAndFlush(SimpleInput.AttributeEquipment);
        else
            Visible = false;
    }

    public void Equip(PlayerEquipMessage message)
    {
        if (!Visible)
            return;
        _equipment.Equip(message);
        _connection?.WriteAndFlush(SimpleInput.AttributeQuietly);
    }

    public void KungFuRefreshed()
    {
        if (!Visible)
            return;
        _connection?.WriteAndFlush(SimpleInput.AttributeQuietly);
    }

    public void Unequip(EquipmentType type)
    {
        if (!Visible)
            return;
        _equipment.Unequip(type);
        _connection?.WriteAndFlush(SimpleInput.AttributeQuietly);
    }

    public void SetConnection(Connection connection)
    {
        _connection = connection;
    }

    public void ShowEquipmentDescription(EquipmentType type, string text)
    {
        if (!Visible)
            return;
        _equipment.ShowEquipmentDescription(type, text);
    }
}