#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Godot;

namespace QnClient.code.map;

public class AtzMapFileParser
{
    public override string ToString()
    {
        return $"{nameof(Height)}: {Height}, {nameof(Width)}: {Width}";
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct Header
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string Magic;
        public int BlockSize;
        public int Width;
        public int Height;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MapCell
    {
        /*TileId : word;
TileNumber: byte;
TileOverId: word;
TileOverNumber: byte;
ObjectId: word;
ObjectNumber: byte;
RoofId: word;
boMove: byte;*/
        public short TileId;
        public byte TileNumber;
        public short TileOverId;
        public byte TileOverNumber;
        public short ObjectId;
        public byte ObjectNumber;
        public short RoofId;
        public byte BoMove;
    }

    private const int BlockSize = 40;


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct MapBlockData
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string MapBlockIdent;
        public int MapChangedCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1600)]
        public MapCell[] MapCells;
    }

    private readonly MapCell[,] _cells;

    private readonly int _height;
        
    private readonly int _width;

    private ISet<int>? _tileOverIds;
    private ISet<int>? _tileIds;
        
    public string Name { get; } 

    public IEnumerable<int> TileIds => _tileIds ??= BuildTextureIds((j, i) => _cells[j, i].TileId);

    private ISet<int> BuildTextureIds(Func<int, int, int> idProvider)
    {
        var tmp = new HashSet<int>();
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                tmp.Add(idProvider.Invoke(j, i));
            }
        }
        return tmp;
    }
    
    public IEnumerable<int> TileOverIds => _tileOverIds ??= BuildTextureIds((j, i) => _cells[j, i].TileOverId);

    private AtzMapFileParser(MapCell[,] coor, int heigh, int width, string name)
    {
        _cells = coor;
        _height = heigh;
        _width = width;
        Name = name;
    }

        
    public void ForeachCell(Vector2I start, Vector2I end, Action<MapCell, int, int> action)
    {
        for (var y = start.Y; y <= end.Y; y++)
        {
            if (y < 0 || y >= Height)
            {
                continue;
            }
            for (var x = start.X; x < end.X; x++)
            {
                if (x < 0 || x >= Width)
                {
                    continue;
                }
                action.Invoke(_cells[y, x], x, y);
            }
        }
    }
    

    private bool IsInRange(int x, int y)
    {
        return x >= 0 && x < _width && y >=0 && y < _width;
    }

    public Vector2I End => new Vector2I(_width, _height);

    private bool IsMovable(int x, int y)
    {
        return IsInRange(x, y) && (_cells[y, x].BoMove & 0x1 ) == 0 && (_cells[y, x].BoMove & 0x2 ) == 0;
    }

    public bool ShouldHideRoof(Vector2I point)
    {
        if (point.X >= _width || point.X < 0 || point.Y >= _height || point.Y < 0)
        {
            return false;
        }
        return _cells[point.Y, point.X].BoMove == 4;
    }
        
    private static T BytesToStructure<T>(byte[] bytes)
    {
        int size = Marshal.SizeOf(typeof(T));
        if (bytes.Length < size)
            size = bytes.Length;
        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.Copy(bytes, 0, ptr, size);
            return (T)Marshal.PtrToStructure(ptr, typeof(T));
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    public int Height => _height;

    public int Width => _width;

    public static AtzMapFileParser? ParseFile(string path)
    {
        if (!Godot.FileAccess.FileExists(path))
        {
            return null;
        }
        using (Godot.FileAccess? fileAccess = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read))
        {
            int len = Marshal.SizeOf(typeof(Header));
            var bytes = fileAccess.GetBuffer(len);
            var header = BytesToStructure<Header>(bytes);
            MapCell[,] coor = new MapCell[header.Height, header.Width];
            if (!header.Magic.StartsWith("ATZMAP2"))
            {
                return null;
            }
            for (int h = 0; h < header.Height / BlockSize; h++)
            {
                for (int w = 0; w < header.Width / BlockSize; w++)
                {
                    bytes = fileAccess.GetBuffer(Marshal.SizeOf<MapBlockData>());
                    var block = BytesToStructure<MapBlockData>(bytes);
                    for (int by = 0; by < BlockSize; by++)
                    {
                        for (int bx = 0; bx < BlockSize; bx++)
                        {
                            coor[h * BlockSize + by, w * BlockSize + bx] = block.MapCells[by * BlockSize + bx];
                        }
                    }
                }
            }
            var name = Path.GetFileName(path);
            return new AtzMapFileParser(coor, header.Height, header.Width, name.Substring(0, name.Length - 4)); // -4 to remove .map
        }
    }

    public bool CanMove(Vector2I coordinate)
    {
        return IsMovable(coordinate.X, coordinate.Y);
    }
}