using System.Text.RegularExpressions;
using Godot;
using NLog;
using QnClient.code.hud;
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
    private Login _login;
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    public override void _Ready()
    {
        GetTree().AutoAcceptQuit = false;
        _hud = GetNode<HUD>("HUD");
        _game = GetNode<Game>("GameViewportContainer/GameViewport/Game");
        _login = GetNode<Login>("Login");
        _login.LoggedIn += OnLoggedIn;
        _login.Exited += Exit;
        SetupConnection();
        AtdLoader.Instance.Load("0");
    }

    private void Exit()
    {
        GetTree().Quit();
    }

    private void OnLoggedIn()
    {
        _hud.SetConnection(_connection);
        _hud.Visible = true;
        _login.QueueFree();
        _game.Start(_connection, _hud);
    }
    
    private async void SetupConnection()
    {
        _connection = await Connection.ConnectTo("127.0.0.1", 9999);
        _login.OnConnected(_connection);
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            Exit();
        }
    }
}