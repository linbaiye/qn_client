using System;
using System.Collections.Generic;
using Godot;
using NLog;
using QnClient.code.entity;

namespace QnClient.code.sprite;

public class AtdLoader
{
    public static readonly AtdLoader Instance = new();

    private static readonly Dictionary<string, List<String>> Cache = new();
    private AtdLoader()
    {
    }

    private const string DirPath = "res://atd/";

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    

    private static string? Decode(byte[] bytes)
    {
        for (int i = 0; i < bytes.Length; i++)
        {
            var b = bytes[i];
            var l = 0x0f & b;
            var h = 0xf0 & b;
            bytes[i] = (byte)((h >> 4) + (l << 4));
        }
        int len = bytes[0] & 0xff;
        if (len <= 0)
        {
            return null;
        }
        return System.Text.Encoding.UTF8.GetString(bytes, 1, len);
    }
    
    // private List<AnimationDescriptor> ConvertToActions(List<string> list)
    // {
    //     List<AtdAction> result = new System.Collections.Generic.List<AtdAction>();
    //
    //     foreach (var str in list)
    //     {
    //         Logger.Debug(str);
    //         var strings = Regex.Replace(str, @"\s+", "").Split(",");
    //         if (string.IsNullOrEmpty(strings[0]) || "Name".Equals(strings[0]))
    //         {
    //             continue;
    //         }
    //         string action = strings[1];
    //         string direction = strings[2];
    //         int frame = strings[3].ToInt();
    //         int frameTime = action.Equals("TURN") ? strings[4].ToInt() * 10 : strings[4].ToInt();
    //         result.Add(new AtdAction(action, direction, frame, frameTime, ToFrameArray(frame, strings)));
    //     }
    //     return result;
    // }
    
            
    public List<string> Load(string name)
    {
        if (!name.EndsWith(".atd"))
        {
            name += ".atd";
        }
        if (Cache.TryGetValue(name, out var result))
        {
            return result;
        }
        FileAccess fileAccess = FileAccess.Open(DirPath + name, FileAccess.ModeFlags.Read);
        List<string> list = new List<string>();
        int cnt = (int)fileAccess.GetLength() / 255;
        for (int i = 0; i < cnt; i++)
        {
            var buffer = fileAccess.GetBuffer(255);
            if (buffer == null)
            {
                break;
            }
            var decoded = Decode(buffer);
            if (decoded != null)
            {
                list.Add(decoded);
            }
        }
        fileAccess.Close();
        Cache.Add(name, list);
        return list;
    }
    
}