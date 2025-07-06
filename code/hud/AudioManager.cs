using System.Threading.Tasks;
using Godot;

namespace QnClient.code.hud;

public partial class AudioManager : Node
{

    private AudioStreamPlayer2D _bgmPlayer;

    private string _currentBgm = "";
    
    private AudioStreamPlayer2D[] _soundPlayers = new AudioStreamPlayer2D[4];

    public override void _Ready()
    {
        _bgmPlayer = GetNode<AudioStreamPlayer2D>("BgmPlayer");
        _bgmPlayer.Finished += ReplayBgm;
        for (int i = 0; i < _soundPlayers.Length; i++)
        {
            _soundPlayers[i] = GetNode<AudioStreamPlayer2D>("SoundPlayer" + (i + 1));
        }
    }
    
    private async void ReplayBgm()
    {
        await Task.Delay(10000);
        _bgmPlayer.Play();
    }


    private static AudioStream? LoadBgmStream(string bgm) {
        var path = "res://bgm/" + bgm + ".mp3";
        if (ResourceLoader.Exists(path)) {
            return ResourceLoader.Load<AudioStreamMP3>(path);
        }
        path = "res://bgm/" + bgm + ".wav";
        if (ResourceLoader.Exists(path)) {
            return ResourceLoader.Load<AudioStreamWav>(path);
        }
        return null;
    }

    public void PlaySound(string entityName, string sound)
    {
        if (string.IsNullOrEmpty(sound))
        {
            return;
        }
        foreach (var t in _soundPlayers)
        {
            if (!t.Playing)
            {
                var stream = LoadSound(sound);
                t.Stream = stream;
                t.Play();
                break;
            }
        }
    }

    private AudioStreamWav? LoadSound(string sound)
    {
        var path = "res://sound/" + sound + ".wav";
        if (ResourceLoader.Exists(path))
        {
            return ResourceLoader.Load<AudioStreamWav>(path);
        }
        return null;
    }
    
    public void PlayBgm(string bgm)
    {
        if (_currentBgm.Equals(bgm))
            return;
        AudioStream? stream = LoadBgmStream(bgm);
        if (stream != null)
        {
            _bgmPlayer.Stop();
            _bgmPlayer.Stream = stream;
            _bgmPlayer.Play();
            _currentBgm = bgm;
        }
    }
}