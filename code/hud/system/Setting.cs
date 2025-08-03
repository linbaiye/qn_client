using System;
using System.Text.Json;
using Godot;

namespace QnClient.code.hud.system;

public partial class Setting : NinePatchRect, ISystemSetting
{
    private CheckBox _bgm;
    
    private CheckBox _sound;
    
    private HSlider _bgmVolume;
    
    private HSlider _soundVolume;
    private Button _close;

    private FileStorage _file;
    
    public event Action<ISystemSetting>? SettingChanged;

    public event Action? ClosePressed;
    
    public override void _Ready()
    {
        _bgm = GetNode<CheckBox>("Bgm");
        _sound = GetNode<CheckBox>("Sound");
        _bgmVolume = GetNode<HSlider>("BgmVolume");
        _soundVolume = GetNode<HSlider>("SoundVolume");
        _close = GetNode<Button>("Close");
        _bgmVolume.ValueChanged += v => OnUpdated();
        _soundVolume.ValueChanged += v => OnUpdated();
        _bgm.Pressed += OnUpdated;
        _sound.Pressed += OnUpdated;
        _close.Pressed += () => ClosePressed?.Invoke();
        _bgm.ButtonPressed = true;
        _sound.ButtonPressed = true;
        _bgmVolume.SetValueNoSignal(100);
        _soundVolume.SetValueNoSignal(100);
        Visible = false;
    }

    private void OnUpdated()
    {
        SettingChanged?.Invoke(this);
    }
    
    private class JsonObject
    {
        public double BgmVolume { get; set; } = 100f;
        public double SoundVolume { get; set; } = 100f;
        public bool BgmEnabled { get; set; } = true;
        public bool SoundEnabled { get; set; } = true;
    }

    public void OnCharacterJoined()
    {
        _file = new FileStorage("setting");
        var content = _file.ReadContent();
        try
        {
            if (!string.IsNullOrEmpty(content))
            {
                JsonObject jsonObject = JsonSerializer.Deserialize<JsonObject>(content);
                _bgm.ButtonPressed = jsonObject.BgmEnabled;
                _sound.ButtonPressed = jsonObject.SoundEnabled;
                _bgmVolume.SetValueNoSignal(jsonObject.BgmVolume);
                _soundVolume.SetValueNoSignal(jsonObject.SoundVolume);
            }
        }
        catch
        {
            _file.Delete();
        }
        SettingChanged?.Invoke(this);
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            if (_file == null)
                return;
            JsonObject jsonObject = new JsonObject();
            jsonObject.BgmEnabled = BgmEnabled;
            jsonObject.SoundEnabled = SoundEnabled;
            jsonObject.BgmVolume = BgmVolume;
            jsonObject.SoundVolume = SoundVolume;
            _file.Save(JsonSerializer.Serialize(jsonObject));
        }
    }

    public bool BgmEnabled => _bgm.ButtonPressed;
    public bool SoundEnabled => _sound.ButtonPressed;
    public double BgmVolume => _bgmVolume.Value;
    public double SoundVolume => _soundVolume.Value;
}