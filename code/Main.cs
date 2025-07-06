using System.Text.RegularExpressions;
using Godot;
using NLog;
using QnClient.code.input;
using QnClient.code.network;
using QnClient.code.sprite;
using HUD = QnClient.code.hud.HUD;

namespace QnClient.code;

public partial class Main : Node
{
    private Connection _connection;

    private HUD _hud;

    private Game _game;
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    public override void _Ready()
    {
        GetTree().AutoAcceptQuit = false;
        _hud = GetNode<HUD>("HUD");
        _game = GetNode<Game>("GameViewportContainer/GameViewport/Game");
        SetupConnection();
        AtdLoader.Instance.Load("0");
    }

    
    private async void SetupConnection()
    {
        _connection = await Connection.ConnectTo("127.0.0.1", 9999);
        _connection.WriteAndFlush(new DebugInput());
        Logger.Debug("Connected");
        _hud.SetConnection(_connection);
        _game.Start(_connection, _hud);
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            _connection.Close();
            GetTree().Quit();
        }
    }
}