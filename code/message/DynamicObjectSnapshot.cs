using System.Collections.Generic;
using Godot;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class DynamicObjectSnapshot
{
    private DynamicObjectSnapshot(string name, long id, string shape, Vector2I coordinate,
        int aniId, int elapsed,
        IEnumerable<Vector2I> coordinates, List<Animate> animations, bool occupying,
        Vector2 offset)
    {
        Name = name;
        Id = id;
        Shape = shape;
        Coordinate = coordinate;
        Elapsed = elapsed;
        Coordinates = coordinates;
        Animations = animations;
        AnimateId = aniId;
        Occupying = occupying;
        Offset = offset;
    }
    
    public int AnimateId { get; }
    public bool Occupying { get; }
    
    public class Animate(bool loop, int start, int end, int id)
    {
        public bool Loop { get; } = loop;
        public int Start { get; } = start;
        public int End { get; } = end;

        public int Id { get; } = id;
    }
    
    public Vector2I Coordinate { get; }
    
    public Vector2 Offset { get; }

    public List<Animate> Animations { get; } 
    public string Name { get; }
    public long Id { get; }
    public string Shape { get; }
    public int Elapsed { get; }
    public IEnumerable<Vector2I> Coordinates { get; }


    public static DynamicObjectSnapshot FromPacket(DynamicObjectSnapshotPacket packet)
    {
        List<Animate> animates = new List<Animate>();
        for (int i = 0; i < packet.AniId.Count; i++)
        {
            animates.Add(new Animate(packet.AniLoop[i], packet.AniStart[i], packet.AniEnd[i], packet.AniId[i]));
        }
        ISet<Vector2I> coordinates = new HashSet<Vector2I>();
        for (int i = 0; i < packet.GuardX.Count; i++)
        {
            coordinates.Add(new Vector2I(packet.GuardX[i], packet.GuardY[i]));
        }
        return new DynamicObjectSnapshot(packet.ViewName, packet.Id, packet.Shape, new Vector2I(packet.X, packet.Y), packet.CurrentAni, packet.CurrentElapsed,
            coordinates, animates, packet.Occupying, new Vector2(packet.OffsetX, packet.OffsetY));
    }
}