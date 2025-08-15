using QnClient.code.input;
using QnClient.code.message;
using QnClient.code.network;

namespace QnClient.code.hud.guild;

public class GuildManager 
{
    private readonly SimpleInputWindow _simpleInputWindow;

    private readonly ApplyKungFuForm _applyKungFuForm;
    
    private readonly Connection _connection;
    
    private int _slotId;

    public GuildManager(SimpleInputWindow simpleInputWindow, Connection connection, ApplyKungFuForm applyKungFuForm)
    {
        _simpleInputWindow = simpleInputWindow;
        _simpleInputWindow.Confirmed += ConfirmCreation;
        _simpleInputWindow.Cancelled += CancelCreation;
        _connection = connection;
        _applyKungFuForm = applyKungFuForm;
        _applyKungFuForm.OnConfirmed += OnFormConfirmed;
    }

    public void ShowCreateGuildWindow(int slotId, string title, string tip)
    {
        _simpleInputWindow.Clear();
        _slotId = slotId;
        _simpleInputWindow.SetTitleTip(title, tip);
    }

    public void HandleApplyKungFuMessage(ApplyKungFuWindowMessage message)
    {
        if (message.IsOpen && !_applyKungFuForm.Visible)
            _applyKungFuForm.Show();
        else if (message.IsClose && _applyKungFuForm.Visible)
            _applyKungFuForm.Hide();
        else if (message.IsMessage)
            _applyKungFuForm.ShowMessage(message.Message);
    }

    private void OnFormConfirmed()
    {
        _connection.WriteAndFlush(_applyKungFuForm.BuildInput());
    }

    private void CancelCreation()
    {
        _connection.WriteAndFlush(CreateGuildInput.Cancel());
        _simpleInputWindow.Clear();
    }

    private void ConfirmCreation()
    {
        if (string.IsNullOrEmpty(_simpleInputWindow.GuildName))
        {
            _simpleInputWindow.ShowError("请输入门派名称");
            return;
        }
        _connection.WriteAndFlush(CreateGuildInput.Confirm(_simpleInputWindow.GuildName, _slotId));
        _simpleInputWindow.Clear();
    }
}