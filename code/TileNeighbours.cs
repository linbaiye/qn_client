using System.Collections.Generic;
using QnClient.code.entity;
using QnClient.code.util;

namespace QnClient.code;

public class TileNeighbours
{
    private readonly List<Dictionary<CreatureDirection, Tile>> _connectableTilesList = new();
    
    private bool Contains(Dictionary<CreatureDirection, Tile> neighbours)
    {
        if (_connectableTilesList.Count == 0)
            return false;
        foreach (var connectableTiles in _connectableTilesList)
        {
            if (connectableTiles.Count != neighbours.Count)
                continue;
            bool found = true;
            foreach (var (dir, tile) in connectableTiles)
            {
                if (neighbours.TryGetValue(dir, out var cur))
                {
                    if (!cur.Equals(tile))
                    {
                        found = false;
                        break;
                    }
                }
                else
                {
                    found = false;
                    break;
                }
            }
            if (found)
                return true;
        }
        return false;
    }


    public Dictionary<CreatureDirection, Tile> SelectNeighbours(Dictionary<CreatureDirection, Tile> currentNeighbours)
    {
        if (currentNeighbours.Count == 0)
            return _connectableTilesList[0];
        Dictionary<CreatureDirection, Tile> mostMatched = new Dictionary<CreatureDirection, Tile>();
        int mostMatchedCount = 0;
        foreach (var dictionary in _connectableTilesList)
        {
            int tmp = 0;
            foreach (var (dir, t) in dictionary)
            {
                if (currentNeighbours.TryGetValue(dir, out var n))
                {
                    tmp += t.Equals(n) ? 1 : 0;
                }
            }
            if (mostMatchedCount < tmp)
            {
                mostMatchedCount = tmp;
                mostMatched = dictionary;
            }
        }
        return mostMatched;
    }
    
    public void AddNeighbours(Dictionary<CreatureDirection, Tile> neighbours)
    {
        if (Contains(neighbours))
            return;
        _connectableTilesList.Add(neighbours);
    }
}