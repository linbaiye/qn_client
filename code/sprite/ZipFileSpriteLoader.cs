using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using NLog;

namespace QnClient.code.sprite;

public class ZipFileSpriteLoader
{

	public static readonly ZipFileSpriteLoader Instance = new ();
	private ZipFileSpriteLoader() {}
	
	
	private static readonly ILogger LOGGER = LogManager.GetCurrentClassLogger();

	private readonly Cache<string, Sprite[]> _cache = new();
	
	private class Cache<TKey, TValue> where TKey : notnull
	{
		private readonly Dictionary<TKey, CacheItem<TValue>> _cache = new();

		public void Store(TKey key, TValue value, TimeSpan expiresAfter)
		{
			_cache[key] = new CacheItem<TValue>(value, expiresAfter);
		}

		public TValue? Get(TKey key)
		{
			if (!_cache.TryGetValue(key, out var cached)) return default(TValue);
			if (DateTimeOffset.Now - cached.Created >= cached.ExpiresAfter)
			{
				_cache.Remove(key);
				return default;
			}
			return cached.Value;
		}
	}
	private class CacheItem<T>
	{
		public CacheItem(T value, TimeSpan expiresAfter)
		{
			Value = value;
			ExpiresAfter = expiresAfter;
		}
		public T Value { get; }
		internal DateTimeOffset Created { get; } = DateTimeOffset.Now;
		internal TimeSpan ExpiresAfter { get; }
	}

	private Vector2 ParseLine(string s)
	{
		if (!s.Contains(','))
		{
			return new Vector2(0, 0);
		}
		var nobrackets = s.Replace("[", "").Replace("]", "");
		var numbers = nobrackets.Split(",");
		return numbers.Length == 2 ? 
			new Vector2(int.Parse(numbers[0].Trim()), int.Parse(numbers[1].Trim())) :
			new Vector2(0, 0);
	}
	
	private Vector2[] ParseVectors(IEnumerable<string> lines)
	{
		return (from line in lines where line.Contains(',') select ParseLine(line)).ToArray();
	}


	public Sprite[] Load(string name)
	{
		var sprites = _cache.Get(name);
		if (sprites != null)
		{
			return sprites;
		}
		using var zipArchive = ZipUtil.LoadZipFile("res://sprites/" + name.ToLower() + ".zip");
		var offsetEntry = zipArchive.GetEntry("offset.txt");
		if (offsetEntry == null)
		{
			throw new FileNotFoundException("Bad atz zip file: " + name);
		}
		var sizeEntry  = zipArchive.GetEntry("size.txt");
		if (sizeEntry == null)
		{
			throw new FileNotFoundException("Bad atz zip file: " + name);
		}
		var vectors = ParseVectors(offsetEntry.ReadAsString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
		var sizes = ParseVectors(sizeEntry.ReadAsString().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
		sprites = new Sprite[vectors.Length];
		for (int i = 0; i < vectors.Length; i++)
		{
			var filename = "000" + i.ToString("D3") + ".png";
			var zipArchiveEntry = zipArchive.GetEntry(filename);
			var texture = zipArchiveEntry?.ReadAsTexture();
			if (texture == null)
			{
				throw new Exception("Invalid atz " + name);
			}

			sprites[i] = new Sprite(texture, vectors[i], sizes[i]);
		}
		LOGGER.Debug("Loaded {0}.", name);
		_cache.Store(name, sprites, TimeSpan.FromMinutes(5));
		return sprites;
	}
}