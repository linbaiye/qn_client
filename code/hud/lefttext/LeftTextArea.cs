using Godot;

namespace QnClient.code.hud.lefttext;

public partial class LeftTextArea : VBoxContainer
{
    
    private LeftTextLabel[] _leftTextLabels = new LeftTextLabel[5];

    public override void _Ready()
    {
        for (int i = 0; i < 5; i++)
        {
            _leftTextLabels[i] = GetNode<LeftTextLabel>("Label" + i);
            _leftTextLabels[i].Timeout += OnLabelTimeout;
        }
    }
    private void OnLabelTimeout(int n)
    {
        _leftTextLabels[n].Clear();
        for (int i = n; i < 4; i++)
        {
            _leftTextLabels[i].Copy(_leftTextLabels[i + 1]);
        }
        _leftTextLabels[4].Clear();
    }

    public void Display(string content)
    {
        for (int i = 0; i < 5; i++)
        {
            if (_leftTextLabels[i].Empty)
            {
                _leftTextLabels[i].SetContent(content);
                return;
            }
        }
        OnLabelTimeout(0);
        _leftTextLabels[4].SetContent(content);
    }
}