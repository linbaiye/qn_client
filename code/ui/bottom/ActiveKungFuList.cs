using Godot;
using QnClient.code.player.character;

namespace QnClient.code.ui.bottom;

public partial class ActiveKungFuList : VBoxContainer
{

    private RichTextLabel[] _kungFuArray = new RichTextLabel[4];
    public override void _Ready()
    {
        for (var i = 0; i < 4; i++)
        {
            _kungFuArray[i] = GetNode<RichTextLabel>("KungFu" + i);
        }
    }

    private string MakeText(string text)
    {
        return "[center]" + text + "[/center]";
    }

    public void Update(ICharacter character)
    {
        foreach (var label in _kungFuArray)
        {
            label.Text = "";
        }
        int index = 0;
        if (character.AttackKungFu != null)
            _kungFuArray[index++].Text = MakeText(character.AttackKungFu.Name);
        if (character.ProtectionKungFu != null)
            _kungFuArray[index++].Text = MakeText(character.ProtectionKungFu.Name);
        if (character.AssistantKungFu != null)
            _kungFuArray[index++].Text = MakeText(character.AssistantKungFu.Name);
        if (character.FootKungFu != null)
            _kungFuArray[index].Text = MakeText(character.FootKungFu.Name);
    }
}