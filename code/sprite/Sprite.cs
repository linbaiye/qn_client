using Godot;

namespace QnClient.code.sprite;

public class Sprite
{
    
    public Sprite(Texture2D texture2D, Vector2 offset, Vector2? originSize = null)
    {
        Texture = texture2D;
        Offset = offset;
        OriginalSize = originSize ?? texture2D.GetSize();
    }

    public Texture2D Texture { get; }

    public Vector2 Offset { get; private set; }

    public Sprite Add(Vector2 off)
    {
        Offset += off;
        return this;
    }
    public Vector2 OriginalSize { get; private set; }
    
}