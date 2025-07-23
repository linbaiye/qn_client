using System.Collections.Generic;
using Godot;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class DynamicObjectSnapshot
{
    private DynamicObjectSnapshot(string name, long id, string shape, Vector2I coordinate, int start, int end, int elapsed,
        IEnumerable<Vector2I> coordinates, bool loop)
    {
        Name = name;
        Id = id;
        Shape = shape;
        Coordinate = coordinate;
        FrameStart = start;
        FrameEnd = end;
        Elapsed = elapsed;
        Coordinates = coordinates;
        Loop = loop;
    }
    
    public Vector2I Coordinate { get; }
    
    public string Name { get; }
    public long Id { get; }
    public string Shape { get; }
    public int FrameStart { get; }
    public int FrameEnd { get; }
    public int Elapsed { get; }
    public bool Loop { get; }
    
    public IEnumerable<Vector2I> Coordinates { get; }


    public static DynamicObjectSnapshot FromPacket(ShowDynamicObjectPacket packet)
    {
        List<Vector2I> coordinates = new List<Vector2I>();
        for (var i = 0; i < packet.GuardX.Count; i++)
        {
            var x = packet.GuardX[i];
            var y = packet.GuardY[i];
            coordinates.Add(new Vector2I(x, y));
        }
        return new DynamicObjectSnapshot(packet.HasName ? packet.Name : "", packet.Id, packet.Shape, 
            new Vector2I(packet.X, packet.Y), packet.Start, packet.End, packet.Elapsed, coordinates, packet.Loop);
    }
    
}