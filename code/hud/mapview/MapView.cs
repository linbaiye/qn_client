using System.Collections.Generic;
using Godot;
using NLog;
using QnClient.code.input;
using QnClient.code.map;
using QnClient.code.message;
using QnClient.code.network;

namespace QnClient.code.hud.mapview;


public partial class MapView : TextureRect, IConnectionAware, ICharacterJoinedAware
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    
    private static readonly Vector2 WindowViewSize = new(1024, 570);

    private MapEntityMarker _characterMarker;

    private Connection _connection;
    
    private IMap _map;
    
    private List<MapEntityMarker> _npcMarkers;
    
    private Vector2I _characterCoordinate;

    private Timer _timer;
    
    public override void _Ready()
    {
        Visible = false;
        _characterMarker = MapEntityMarker.Create("");
        _characterMarker.RemoveThemeStyleboxOverride("panel");
        _characterMarker.AddThemeStyleboxOverride("panel", new StyleBoxFlat() { BgColor = new Color("08f5a5") });
        AddChild(_characterMarker);
        _npcMarkers = new List<MapEntityMarker>();
        _timer = GetNode<Timer>("Timer");
        _timer.Timeout += GetNpcPositions;
    }


    private void GetNpcPositions()
    {
        _connection?.WriteAndFlush(RealmInput.GetNpcCoordinates);
    }

    private void CloseView()
    {
        Visible = false;
        ClearNpcs();
    }
    
    
    private Vector2 ComputeImagePosition(Vector2 imageSize)
    {
        return (WindowViewSize - imageSize) / 2;
    }

    public void SetMap(IMap map)
    {
        _map = map;
    }

    public void UpdateCharacterCoordinate(Vector2I characterCoordinate)
    {
        _characterCoordinate = characterCoordinate; 
        if (Visible)
            _characterMarker.Position = CharPosition;
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey && eventKey.Pressed && eventKey.Keycode == Key.M)
        {
            Toggle();
            GetViewport().SetInputAsHandled();
        }
    }

    private void Toggle()
    {
        if (Texture == null)
            return;
        ClearNpcs();
        if (Visible)
        {
            Visible = false;
            _timer.Stop();
        }
        else
        {
            _characterMarker.Position = CharPosition;
            GetNpcPositions();
            _timer.Start(1);
            Visible = true;
        }
    }

    private void ClearNpcs()
    {
        foreach (var creatureView in _npcMarkers)
        {
            RemoveChild(creatureView);
            creatureView.QueueFree();
        }
        _npcMarkers.Clear();
    }
    
    private Vector2 ComputeCreaturePosition(Vector2I coordinate, Vector2 markerSize)
    {
        var x = coordinate.X / (float)_map.MapSize.X;
        var y = coordinate.Y / (float)_map.MapSize.Y;
        var mapImageSize = Texture.GetSize();
        return new Vector2(mapImageSize.X * x - markerSize.X / 2, mapImageSize.Y * y - markerSize.Y / 2);
    }

    private Vector2 CharPosition => ComputeCreaturePosition(_characterCoordinate, _characterMarker.Size);

    public void ShowCoordinateAndName(InteractablePositionNameMessage nameMessage)
    {
        if (!Visible)
        {
            return;
        }
        var positions = nameMessage.NpcPositions;
        foreach (var npcPosition in positions)
        {
            bool found = false;
            foreach (var mapEntityMarker in _npcMarkers)
            {
                if (mapEntityMarker.Id == npcPosition.Id)
                {
                    mapEntityMarker.Position = ComputeCreaturePosition(npcPosition.Coordinate, mapEntityMarker.Size);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                var mapCreatureView = MapEntityMarker.Create(npcPosition.Name, npcPosition.Id);
                AddChild(mapCreatureView);
                mapCreatureView.Position = ComputeCreaturePosition(npcPosition.Coordinate, mapCreatureView.Size);
                _npcMarkers.Add(mapCreatureView);
            }
        }
    }

    public void SetConnection(Connection connection)
    {
        _connection = connection;
    }

    private void ChangeMap(string mapName)
    {
        var path = "res://maps/" + mapName + ".bmp";
        if (!ResourceLoader.Exists(path) || ResourceLoader.Load(path) is not Texture2D texture)
        {
            Texture = null;
            return;
        }
        ClearNpcs();
        Texture = texture;
        Size = texture.GetSize();
        SetPosition((WindowViewSize - texture.GetSize()) / 2);
    }

    public void OnPlayerTeleported(TeleportMessage message)
    {
        ChangeMap(message.MapFile);
        _characterCoordinate = message.Coordinate;
        CloseView();
    }

    public void OnCharacterJoined(JoinRealmMessage message)
    {
        try
        {
            ChangeMap(message.MapFile);
            _characterMarker.SetEntityIdName(message.Id, message.Name);
            _characterCoordinate = message.Coordinate;
        }
        catch
        {
            // ignored
        }
    }
}