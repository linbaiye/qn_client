using Godot;
using NLog;
using QnClient.code.entity;
using QnClient.code.map;
using QnClient.code.message;
using QnClient.code.network;
using QnClient.code.player;
using QnClient.code.ui;
using Character = QnClient.code.player.character.Character;

namespace QnClient.code;

public partial class Game : Node2D
{
    private AtzMap _map;

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    private Character _character;

    private readonly EntityManager _entityManager = new();

    private Connection _connection;

    private HUD _hud;

    public override void _Ready()
    {
        var groundLayer = GetNode<GroundLayer>("GroundLayer");
        var overGroundLayer = GetNode<OverGroundLayer>("OverGroundLayer");
        var objectLayer = GetNode<ObjectLayer>("ObjectLayer");
        var rootLayer = GetNode<RoofLayer>("RoofLayer");
        _character = GetNode<Character>("Character");
        Visible = false;
        _map = new AtzMap(overGroundLayer, groundLayer, objectLayer, rootLayer);
    }
    


    private void AddEntity(AbstractEntity entity, IEntityMessage message)
    {
        AddChild(entity);
        entity.OnEntityEvent += _map.HandleEntityEvent;
        entity.OnEntityEvent += _entityManager.HandleEntityEvent;
        entity.HandleEntityMessage(message);
        _entityManager.Add(entity);
    }


    private void HandleMessages()
    {
        if (_connection == null)
            return;
        var messages = _connection.DrainMessages();
        foreach (var msg in messages)
        {
            switch (msg)
            {
                case JoinRealmMessage message:
                    _map.Draw(message.MapFile, message.ResourceName);
                    _character.Initialize(message, _connection, _map);
                    _entityManager.Add(_character);
                    Visible = true;
                    break;
                case NpcSnapshot snapshot:
                    AddEntity(Monster.Create(), snapshot);
                    break;
                case PlayerSnapshot playerSnapshot:
                    AddEntity(Player.Create(), playerSnapshot);
                    break;
                case IEntityMessage entityMessage:
                    _entityManager.Find(entityMessage.Id)?.HandleEntityMessage(entityMessage);
                    break;
                case IHUDMessage hudMessage:
                    hudMessage.Accept(_hud);
                    break;
            }
        }
    }

    public override void _Process(double delta)
    {
        HandleMessages();
    }

    public void Start(Connection connection, HUD hud)
    {
        _connection = connection;
        _hud = hud;
        _character.OnEntityEvent += _hud.CharacterEventHandler;
    }
}