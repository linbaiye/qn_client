using System;
using Godot;
using NLog;
using QnClient.code.entity;
using QnClient.code.input;
using QnClient.code.map;
using QnClient.code.message;
using QnClient.code.network;
using QnClient.code.player;
using Character = QnClient.code.player.character.Character;
using HUD = QnClient.code.hud.HUD;

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
    


    private void AddEntity(AbstractCreature creature, IEntityMessage message)
    {
        AddChild(creature);
        creature.OnEntityEvent += _map.HandleEntityEvent;
        creature.OnEntityEvent += _entityManager.HandleEntityEvent;
        creature.HandleEntityMessage(message);
        creature.AttackTriggered += id => _connection.WriteAndFlush(new AttackInput(id));
        _entityManager.Add(creature);
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
                    _character.OnEntityEvent += _map.HandleEntityEvent;
                    _entityManager.Add(_character);
                    Visible = true;
                    break;
                case NpcSnapshot snapshot:
                    AddEntity(Npc.Create(), snapshot);
                    break;
                case PlayerSnapshot playerSnapshot:
                    AddEntity(Player.Create(), playerSnapshot);
                    break;
                case IEntityMessage entityMessage:
                    _entityManager.Find(entityMessage.Id)?.HandleEntityMessage(entityMessage);
                    break;
            }
            if (msg is IHUDMessage hudMessage)
            {
                hudMessage.Accept(_hud);
            }
        }
    }

    public override void _Process(double delta)
    {
        try
        {
            HandleMessages();
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to handle messages.");
        }
    }

    public void Start(Connection connection, HUD hud)
    {
        _connection = connection;
        _hud = hud;
        _character.OnEntityEvent += _hud.CharacterEventHandler;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventKey eventKey || eventKey.Pressed)
        {
            return;
        }

        switch (eventKey.Keycode)
        {
            case Key.Key1:
                _character.HandleEntityMessage(CreatureSayMessage.Test(_character, "雷剑"));
                break;
            case Key.Key2:
                _character.HandleEntityMessage(CreatureSayMessage.Test(_character, "回转狂天飞"));
                break;
            case Key.Key3:
                _character.HandleEntityMessage(CreatureSayMessage.Test(_character, "雷震子： 你是没死过么？"));
                break;
            case Key.Key4:
                _character.HandleEntityMessage(CreatureSayMessage.Test(_character, "雷震子： 用户在使用cherry键盘的时候如果想要关闭f1到f12的功能键的话，是无法做到的，只能关闭功能键的热键功能，无法关闭其中所有的功能。"));
                break;
        }
        
    }
}