using Godot;
using QnClient.code.input;
using QnClient.code.message;
using QnClient.code.network;

namespace QnClient.code.hud;

public partial class Quest : NinePatchRect, IConnectionAware
{

    private Button _close;

    private Label _npcName;
    
    private Label _abstraction;
    private Label _description;
    private Button _submit;
    private string _questName;

    private long _npcId;

    private Connection _connection;

    public override void _Ready()
    {
        _close = GetNode<Button>("Close");
        _npcName = GetNode<Label>("NpcName");
        _abstraction = GetNode<Label>("Abstraction");
        _description = GetNode<Label>("Description");
        _submit = GetNode<Button>("Submit");
        _submit.Pressed += Submit;
        GetNode<Button>("Close").Pressed += () => Visible = false;
        Visible = false;
    }

    private void Submit()
    {
        _connection?.WriteAndFlush(new SubmitQuestInput(_npcId, _questName));
        Visible = false;
    }

    public void Show(ShowQuestMessage message)
    {
        _npcId = message.Id;
        _npcName.Text = message.NpcName;
        _abstraction.Text = message.Abstraction;
        _description.Text = message.Description;
        _submit.Text = message.SubmitText;
        _questName = message.QuestName;
        Visible = true;
    }

    public void SetConnection(Connection connection)
    {
        _connection = connection;
    }
}