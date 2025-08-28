namespace QnClient.code;

public class ColorTile
{

    private int _sourceId;

    private const int Up = 0;
    private const int Right = 1;
    private const int Down = 2;
    private const int Left = 3;

    private EdgeColor[] _edgeColors;

    public ColorTile(int sourceId, EdgeColor[] colors)
    {
        _sourceId = sourceId;
        _edgeColors = colors;
    }

    public bool CanTileDownTo(ColorTile another)
    {
        return _edgeColors[Up] == another._edgeColors[Down];
    }
    
    public bool CanTileUpTo(ColorTile another)
    {
        return _edgeColors[Down] == another._edgeColors[Up];
    }
    
}