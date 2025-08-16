using System.Threading.Tasks;
using Godot;
using QnClient.code.hud.system;
using QnClient.code.message;

namespace QnClient.code.hud;

public partial class AudioManager : Node, ICharacterJoinedAware
{

    private AudioStreamPlayer2D _bgmPlayer;

    private string _currentBgm = "";
    
    private AudioStreamPlayer2D[] _soundPlayers = new AudioStreamPlayer2D[8];

    private bool _settingReceived = false;
    
    public override void _Ready()
    {
        _bgmPlayer = GetNode<AudioStreamPlayer2D>("BgmPlayer");
        _bgmPlayer.Finished += ReplayBgm;
        for (int i = 0; i < _soundPlayers.Length; i++)
        {
            _soundPlayers[i] = GetNode<AudioStreamPlayer2D>("SoundPlayer" + (i + 1));
        }
    }

    private float ToDb(double v)
    {
        return (float)Mathf.LinearToDb(v / 100);
    }
    
    private async void ReplayBgm()
    {
        await Task.Delay(10000);
        _bgmPlayer.Play();
    }
    

    public void OnSystemSettingChanged(ISystemSetting setting)
    {
        _settingReceived = true;
        _bgmPlayer.VolumeDb = ToDb(setting.BgmEnabled ? setting.BgmVolume : 0);
        foreach (var soundPlayer in _soundPlayers)
            soundPlayer.VolumeDb = ToDb(setting.SoundEnabled ? setting.SoundVolume : 0);
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

    public void PlaySound(string sound)
    {
        if (string.IsNullOrEmpty(sound))
        {
            return;
        }

        foreach (var t in _soundPlayers)
        {
            if (!t.HasMeta("name"))
                continue;
            if (((string)t.GetMeta("name")).Equals(sound) && t.IsPlaying())
            {
                return;
            }
        }

        foreach (var t in _soundPlayers)
        {
            if (t.IsPlaying()) continue;
            var stream = LoadSound(sound);
            if (stream != null)
            {
                t.Stream = stream;
                t.Play();
                t.SetMeta("name", sound);
            }
            break;
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
        else
        {
            _currentBgm = null;
            _bgmPlayer.Stop();
        }
    }

    public void OnCharacterJoined(JoinRealmMessage message)
    {
        if (!_settingReceived)
            _bgmPlayer.VolumeDb = ToDb(0);
        PlayBgm(message.Bgm);
    }
}