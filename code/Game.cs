using Godot;
using NLog;
using QnClient.code.input;
using QnClient.code.map;
using QnClient.code.message;
using QnClient.code.network;
using QnClient.code.player;

namespace QnClient.code;

public partial class Game : Node2D
{
    private AtzMap _map;

    private Connection _connection;
    
    private static readonly ILogger Logger  = LogManager.GetCurrentClassLogger();

    private Character _character;
    
    public override void _Ready()
    {
        var groundLayer = GetNode<GroundLayer>("GroundLayer");
        var overGroundLayer = GetNode<OverGroundLayer>("OverGroundLayer");
        var objectLayer = GetNode<ObjectLayer>("ObjectLayer");
        var rootLayer = GetNode<RoofLayer>("RoofLayer");
        _character = GetNode<Character>("Character");
        Visible = false;
        _map = new AtzMap(overGroundLayer, groundLayer, objectLayer, rootLayer);
        _character.SetMap(_map);
        SetupConnection();
    }


    private void HandleMessages()
    {
        if (_connection == null)
            return;
        var messages = _connection.DrainMessages();
        foreach (var msg in messages)
        {
            if (msg is JoinRealmMessage message)
            {
                _map.Draw(message.MapFile, message.ResourceName);
                _character.Handle(message);
                Visible = true;
            }
        }
    }

    public override void _Process(double delta)
    {
        HandleMessages();
    }

    private async void SetupConnection()
    {
        _connection = await Connection.ConnectTo("127.0.0.1", 9999);
        _connection.WriteAndFlush(new DebugInput());
    }
}