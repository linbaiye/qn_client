using System;
using Godot;
using NLog;
using QnClient.code.entity;
using QnClient.code.entity.@event;
using QnClient.code.input;
using QnClient.code.map;
using QnClient.code.message;
using QnClient.code.network;
using QnClient.code.player;
using QnClient.code.util;
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
    


    private void AddCreature(AbstractCreature creature, IEntityMessage message)
    {
        AddChild(creature);
        creature.OnEntityEvent += _map.HandleEntityEvent;
        creature.OnEntityEvent += _entityManager.HandleEntityEvent;
        creature.ShootEvent += HandleShoot;
        creature.HandleEntityMessage(message);
        creature.AttackTriggered += id => _connection.WriteAndFlush(new AttackInput(id));
        creature.Clicked += id => _connection.WriteAndFlush(new ClickEntityInput(id));
        _entityManager.Add(creature);
    }

    private void AddGroundItem(GroundItemSnapshot snapshot)
    {
        var groundItem = GroundItem.Create();
        AddChild(groundItem);
        groundItem.Init(snapshot);
        groundItem.OnEntityEvent += _entityManager.HandleEntityEvent;
        groundItem.Picked += i => _connection.WriteAndFlush(i);
        _entityManager.Add(groundItem);
    }


    private void AddDynamicObject(DynamicObjectSnapshot snapshot)
    {
        var obj = DynamicObject.Create();
        AddChild(obj);
        obj.OnEntityEvent += _entityManager.HandleEntityEvent;
        obj.OnEntityEvent += _map.HandleEntityEvent;
        obj.Initialize(snapshot);
        obj.AttackTriggered += id => _connection.WriteAndFlush(new AttackInput(id));
        _entityManager.Add(obj);
    }

    private void OnInventoryItemDropped(int slot)
    {
        var mousePosition = GetLocalMousePosition();
        var entity = _entityManager.FindFirst<IEntity>(e => e.IsCoveringPosition(mousePosition));
        if (entity != null)
        {
            _connection.WriteAndFlush(new DropOnEntityInput(entity.Id, slot));
        }
        else
        {
            _connection.WriteAndFlush(new DropItemInput(slot, mousePosition.ToCoordinate()));
        }
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
                    _character.ShootEvent += HandleShoot;
                    _entityManager.Add(_character);
                    Visible = true;
                    break;
                case NpcSnapshot snapshot:
                    AddCreature(Npc.Create(), snapshot);
                    break;
                case PlayerSnapshot playerSnapshot:
                    AddCreature(Player.Create(), playerSnapshot);
                    break;
                case GroundItemSnapshot itemSnapshot:
                    AddGroundItem(itemSnapshot);
                    break;
                case DynamicObjectSnapshot dynamicObjectSnapshot:
                    AddDynamicObject(dynamicObjectSnapshot);
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

    private void HandleShoot(ShootEvent shootEvent)
    {
        var entity = _entityManager.Find<ICreature>(shootEvent.TargetId);
        if (entity == null)
            return;
        var projectile = Projectile.Create(shootEvent.Start, entity.ProjectileAimPoint, shootEvent.Sprite, shootEvent.FlyMillis);
        AddChild(projectile);
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
        _hud.InventoryItemDropped += OnInventoryItemDropped;
    }

    

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventKey eventKey || eventKey.Pressed)
        {
            return;
        }

        switch (eventKey.Keycode)
        {
            case Key.E:
                //AddDynamicObject(DynamicObjectSnapshot.Test());
                break;
            case Key.Q:
                _character.HandleEntityMessage(CreatureSayMessage.Test(_character, "雷震子： 用户在使用cherry键盘的时候如果想要关闭f1到f12的功能键的话，是无法做到的，只能关闭功能键的热键功能，无法关闭其中所有的功能。"));
                break;
            case Key.Key0:
            case Key.Key1:
            case Key.Key2:
            case Key.Key3:
            case Key.Key4:
            case Key.Key5:
            case Key.Key6:
            case Key.Key7:
                var dir = (CreatureDirection)(int)eventKey.Keycode - (int)Key.Key0;
                _character.FollowRope(dir);
                //_dynamicObject.Play(@event);
                break;
        }
    }
}