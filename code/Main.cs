using Godot;
using NLog;
using QnClient.code.hud;
using QnClient.code.message;
using QnClient.code.network;
using QnClient.code.sprite;
using HUD = QnClient.code.hud.HUD;

namespace QnClient.code;

public partial class Main : Node
{
    private IConnection _connection;

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
        //_connection = await Connection.ConnectTo("193.112.251.231", 9999);
        if (Game.Dev.SERVER == Game.DEV_MODE)
        {
            _connection = await Connection.ConnectTo("192.168.139.128", 9999);
            _login.OnConnected(_connection, true);
        } 
        else if (Game.Dev.SERVER == Game.DEV_MODE)
        {
            _connection = await Connection.ConnectTo("192.168.139.128", 9999);
            _login.OnConnected(_connection);
        } 
        else if (Game.Dev.MAP == Game.DEV_MODE)
        {
            var c = new DevConnection();
            var joinRealmMessage = JoinRealmMessage.DebugMap();
            c.Add(joinRealmMessage);
            c.Add(SyncActiveKungFuListMessage.Dev(joinRealmMessage.Id));
            _connection = c;
            _login.OnConnected(_connection, true);
        }
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            Exit();
        }
    }
}