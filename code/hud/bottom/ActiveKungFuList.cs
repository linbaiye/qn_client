using Godot;
using QnClient.code.message;

namespace QnClient.code.hud.bottom;

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

    public void SetAttackKungFu(string name)
    {
        _kungFuArray[0].Text = MakeText(name);
    }
    
    public void SyncActiveKungFu(SyncActiveKungFuListMessage message)
    {
        foreach (var label in _kungFuArray)
        {
            label.Text = "";
        }
        int index = 0;
        _kungFuArray[index++].Text = MakeText(message.AttackKungFu);
        if (!string.IsNullOrEmpty(message.ProtectionKungFu)) 
            _kungFuArray[index++].Text = MakeText(message.ProtectionKungFu);
        if (!string.IsNullOrEmpty(message.BreathKungFu)) 
            _kungFuArray[index++].Text = MakeText(message.BreathKungFu);
        if (!string.IsNullOrEmpty(message.AssistantKungFu))
            _kungFuArray[index++].Text = MakeText(message.AssistantKungFu);
        if (message.FootKungFu != null)
            _kungFuArray[index].Text = MakeText(message.FootKungFu.Name);
    }
}