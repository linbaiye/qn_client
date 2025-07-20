using Godot;
using QnClient.code.input;
using QnClient.code.message;

namespace QnClient.code.hud.npc;

public partial class NpcMainMenu : AbstractNpcMenu
{
    
    private VBoxContainer _submenuItems;
    
    private Button[] _submenuButtons;

    
    public override void _Ready()
    {
        _submenuItems = GetNode<VBoxContainer>("SubmenuItems");
        _submenuButtons = new Button[_submenuItems.GetChildCount()];
        for (int i = 0; i < _submenuButtons.Length; i++)
        {
            _submenuButtons[i] = _submenuItems.GetNode<Button>("Button" + i);
            int number = i;
            _submenuButtons[i].Pressed += () => OnAbilityClicked(number);
        }
    }

    private void OnAbilityClicked(int n)
    {
        SendMessage(new ClickNpcAbilityInput(NpcId, _submenuButtons[n].Text));
    }

    public void Show(NpcMenuMessage message)
    {
        SetFields(message.Name, message.Id, message.Greetings, message.Sprite, message.Image);
        for (int i = 0; i < message.SupportedActions.Count; i++)
        {
            _submenuButtons[i].Text = message.SupportedActions[i];
        }
        Visible = true;
    }
}