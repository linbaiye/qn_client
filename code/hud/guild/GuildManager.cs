using QnClient.code.input;
using QnClient.code.network;

namespace QnClient.code.hud.guild;

public class GuildManager 
{
    private readonly SimpleInputWindow _simpleInputWindow;
    
    private readonly Connection _connection;
    
    private int _slotId;

    public GuildManager(SimpleInputWindow simpleInputWindow, Connection connection)
    {
        _simpleInputWindow = simpleInputWindow;
        _simpleInputWindow.Confirmed += ConfirmCreation;
        _simpleInputWindow.Cancelled += CancelCreation;
        _connection = connection;
    }

    public void ShowCreateGuildWindow(int slotId, string title, string tip)
    {
        _simpleInputWindow.Clear();
        _slotId = slotId;
        _simpleInputWindow.SetTitleTip(title, tip);
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