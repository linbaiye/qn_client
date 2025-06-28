using System.IO;
using System.IO.Compression;
using System.Text;
using Godot;
using NLog;
using FileAccess = Godot.FileAccess;

namespace QnClient.code.sprite;

public static class ZipUtil
{
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
    public static string ReadAsString(this ZipArchiveEntry entry)
    {
        using var osr = new StreamReader(entry.Open(), Encoding.Default);
        return osr.ReadToEnd();
    }
    
    public static ImageTexture? ReadAsTexture(this ZipArchiveEntry entry)
    {
        using var stream = entry.Open();
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        var image = new Image();
        var error = Error.Failed;
        if (entry.FullName.EndsWith(".png"))
        {
            error = image.LoadPngFromBuffer(ms.ToArray());
        }
        else if (entry.FullName.EndsWith(".bmp"))
        {
           error = image.LoadBmpFromBuffer(ms.ToArray());
        }
        if (error != Error.Ok)
        {
            Log.Debug("Failed to load {}.", entry.FullName);
            
        }
        return error == Error.Ok ? ImageTexture.CreateFromImage(image) : null;
    }

    public static string ToString(byte[] bytes)
    {
        return Encoding.UTF8.GetString(bytes);
    }

    public static ZipArchive LoadZipFile(string resourcePath)
    {
        using var fa = FileAccess.Open(resourcePath, FileAccess.ModeFlags.Read);
        var bytes = fa.GetBuffer((int)fa.GetLength());
        Stream stream = new MemoryStream(bytes);
        return new ZipArchive(stream);
    }

    public static ImageTexture? ToTexture(byte[] bytes)
    {
        var image = new Image();
        var error = image.LoadPngFromBuffer(bytes);
        return error == Error.Ok ? ImageTexture.CreateFromImage(image) : null;
    }
}