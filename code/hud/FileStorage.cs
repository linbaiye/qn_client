using Godot;
using FileAccess = Godot.FileAccess;

namespace QnClient.code.hud;
public class FileStorage
{
    private readonly string _filePath;

    public static string Space = "";

    public FileStorage(string filename)
    {
        var dir = "user://data/" + Space;
        _filePath =  dir + "/" + filename;
        if (!DirAccess.DirExistsAbsolute(dir))
            DirAccess.MakeDirRecursiveAbsolute(dir);
    }


    public void Save(string content)
    {
        var fileAccess = FileAccess.Open(_filePath, FileAccess.ModeFlags.Write);
        fileAccess.StoreString(content);
        fileAccess.Close();
    }

    public void Delete()
    {
        DirAccess.RemoveAbsolute(_filePath);
    }

    public string? ReadContent()
    {
        if (!FileAccess.FileExists(_filePath))
        {
            return null;
        }
        FileAccess fileAccess = FileAccess.Open(_filePath, FileAccess.ModeFlags.Read);
        var str = fileAccess.GetAsText();
        fileAccess.Close();
        return str;
    }
}