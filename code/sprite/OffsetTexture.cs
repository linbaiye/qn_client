using Godot;

namespace QnClient.code.sprite;

public struct OffsetTexture(Texture2D texture2D, Vector2 offset, Vector2? originSize = null)
{
    public Texture2D Texture { get; } = texture2D;

    public Vector2 Offset { get; set; } = offset;

    public Vector2 OriginalSize { get; private set; } = originSize ?? texture2D.GetSize();
}