using Godot;
using QnClient.code.message;

namespace QnClient.code.hud.bottom;

public partial class ActiveKungFuList : VBoxContainer
{

    private BlinkingLabel[] _kungFuArray = new BlinkingLabel[4];
    public override void _Ready()
    {
        for (var i = 0; i < 4; i++)
        {
            _kungFuArray[i] = GetNode<BlinkingLabel>("KungFu" + i);
        }
    }


    public void SetAttackKungFu(string name)
    {
        _kungFuArray[0].SetKungFuName(name);
    }

    public void BlinkKungFu(string name)
    {
        foreach (var activeKungFuLabel in _kungFuArray)
        {
            if (activeKungFuLabel.BlinkIfMatches(name))
            {
                break;
            }
        }
    }
    
    public void SyncActiveKungFu(SyncActiveKungFuListMessage message)
    {
        foreach (var label in _kungFuArray)
        {
            label.SetKungFuName("");
        }
        int index = 0;
        _kungFuArray[index++].SetKungFuName(message.AttackKungFu);
        if (!string.IsNullOrEmpty(message.ProtectionKungFu)) 
            _kungFuArray[index++].SetKungFuName(message.ProtectionKungFu);
        if (!string.IsNullOrEmpty(message.BreathKungFu)) 
            _kungFuArray[index++].SetKungFuName(message.BreathKungFu);
        if (!string.IsNullOrEmpty(message.AssistantKungFu))
            _kungFuArray[index++].SetKungFuName(message.AssistantKungFu);
        if (message.FootKungFu != null)
            _kungFuArray[index].SetKungFuName(message.FootKungFu.Name);
    }
}