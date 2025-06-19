using Godot;

namespace QnClient.code.map;

public class MapObject(Texture2D[] texture, Vector2 offset, int id)
{
    public Texture2D[] Textures { get; } = texture;

    public Vector2 Offset { get;  } = offset;

    private int Id { get; } = id;

    public string Name(int x, int y)
    {
        return "obj_" + Id + "_" + x + "_" + y;
    }
}