using Godot;
using NLog;
using QnClient.code.entity;
using QnClient.code.player;
using QnClient.code.player.character;

namespace QnClient.code.map;

public class DevMap : IMap
{
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
    public bool CanMove(Vector2I coordinate)
    {
        return true;
    }

    public Vector2I MapSize => End;
    public Vector2I Start => new Vector2I(0, 0);
    public Vector2I End => new Vector2I(100000, 100000);
    
    public string Name  => "DevMap";
    
    public void HandleEntityEvent(IEntityEvent entityEvent)
    {
        if (entityEvent.Source is Character character)
            Log.Debug("Position {}.", character.Position);
    }

    public void Load(string name, string textureResourceName, Vector2I coordinate)
    {
        
    }
}