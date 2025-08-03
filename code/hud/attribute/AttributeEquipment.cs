using Godot;
using QnClient.code.message;

namespace QnClient.code.hud.attribute;

public partial class AttributeEquipment : NinePatchRect
{
    private Label[] _attributes = new Label[21];
    private Button _close;
    private Label _name;
    private Label _age;
    private Equipment _equipment;
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
    }

    public void ShowAttributeEquipment(AttributeEquipmentMessage message)
    {
        for (int i = 0; i < message.Attributes.Length; i++)
        {
            _attributes[i].Text = message.Attributes[i];
        }
        _name.Text = message.Name;
        _age.Text = message.Age;
        Visible = true;
    }
    
}