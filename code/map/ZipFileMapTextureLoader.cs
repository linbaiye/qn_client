using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;
using NLog;
using QnClient.code.sprite;

namespace QnClient.code.map;

public class ZipFileMapTextureLoader
{
	private const string ObjectDirName = "object";
	private const string TileDirName = "tile";
	private const string RoofDirName = "roof";

	private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

	public static readonly ZipFileMapTextureLoader Instance = new();
	private ZipFileMapTextureLoader() { }

	private string MakePath(string name)
	{
		return "res://maps/" + name + ".zip";
	}

	public IDictionary<int, Texture2D> LoadTiles(string fileName)
	{
		long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
		var zipPath = MakePath(fileName);
		var texture2Ds = new Dictionary<int, Texture2D>();
		try
		{
			using var zipArchive = ZipUtil.LoadZipFile(zipPath);
			var tilepath = TileDirName + "/";
			foreach (var entry in zipArchive.Entries)
			{
				if (!entry.FullName.StartsWith(tilepath))
					continue;
				var imageTextureFromEntry = entry.ReadAsTexture();
				if (imageTextureFromEntry != null)
				{
					var name = Path.GetFileNameWithoutExtension(entry.FullName);
					texture2Ds.Add(name.ToInt(), imageTextureFromEntry);
				}
			}
			long end = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			Logger.Debug("Took {0} to read tiles for {1}.", end - start, fileName);
			return texture2Ds;
		}
		catch (Exception e)
		{
			GD.PrintErr(e);
			throw;
		}
	}

	private struct Object2Json
	{
		public Object2Json()
		{
			Width = 0;
			Height = 0;
			X = 0;
			Y = 0;
			Number = 1;
			Delay = 0;
		}

		public byte Version => 2;

		public int Width { get; set; }

		public int Height { get; set; }

		public int X { get; set; }
		public int Y { get; set; }

		public int Number { get; set; }

		public int Delay { get; set; }

		public static Object2Json FromJsonString(string jsonString)
		{
			return JsonSerializer.Deserialize<Object2Json>(jsonString);
		}
	}

	private int Number(string path)
	{
		var strings = path.Split("/");
		return strings[^2].ToInt();
	}

	private IDictionary<int, MapObject> LoadObjects(string mapName, string dirName)
	{
		IDictionary<int, MapObject> dictionary = new Dictionary<int, MapObject>();
		var path = MakePath(mapName);
		Logger.Debug("Loading zip path " + path);
		try
		{
			using var zipArchive = ZipUtil.LoadZipFile(path);
			IDictionary<int, IDictionary<int, Texture2D>> textures = new Dictionary<int, IDictionary<int, Texture2D>>();
			IDictionary<int, Object2Json> structJson = new Dictionary<int, Object2Json>();
			var objpath = dirName + "/";
			long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			foreach (var entry in zipArchive.Entries)
			{
				if (!entry.FullName.StartsWith(objpath))
					continue;
				if (entry.FullName.EndsWith("struct.json"))
				{
					int number = Number(entry.FullName);
					structJson.Add(number, JsonSerializer.Deserialize<Object2Json>(entry.ReadAsString()));
				}
				else if (entry.FullName.EndsWith(".png"))
				{
					int number = Number(entry.FullName);
					var imageTextureFromEntry = entry.ReadAsTexture();
					if (imageTextureFromEntry == null)
					{
						continue;
					}
					if (!textures.ContainsKey(number))
						textures.Add(number, new Dictionary<int, Texture2D>());
					if (textures.TryGetValue(number, out var map))
					{
						var name = Path.GetFileNameWithoutExtension(entry.FullName);
						map.Add(name.ToInt(), imageTextureFromEntry);
					}
				}
			}

			foreach (var kv in structJson)
			{
				var index = kv.Key;
				var json = kv.Value;
				if (!textures.TryGetValue(index, out var map))
				{
					continue;
				}
				var texture2Ds = new Texture2D[json.Number];
				for (int i = 0; i < json.Number; i++)
				{
					if (map.TryGetValue(i, out var t))
					{
						texture2Ds[i] = t;
					}
				}
				dictionary.TryAdd(index, new MapObject(texture2Ds, new Vector2(json.X, json.Y), index));
			}
			long end = DateTimeOffset.Now.ToUnixTimeMilliseconds();
			Logger.Debug("Took {0} to read {1} for {2}.", end - start, dirName, mapName);
			return dictionary;
		}
		catch (Exception e)
		{
			GD.PrintErr(e);
			throw;
		}
	}

	public IDictionary<int, MapObject> LoadObjects(string name)
	{
		return LoadObjects(name, ObjectDirName);
	}

	public bool HasRoof(string mapName)
	{
		var path = MakePath(mapName);
		using var zipArchive = ZipUtil.LoadZipFile(path);
		return zipArchive.Entries.Any(e => e.FullName.StartsWith(RoofDirName + "/"));
	}

	public IDictionary<int, MapObject> LoadRoof(string mapName)
	{
		if (!HasRoof(mapName))
		{
			return new Dictionary<int, MapObject>();
		}
		return LoadObjects(mapName, RoofDirName);
	}
    
}