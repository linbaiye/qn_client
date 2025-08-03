using System;
using Godot;
using QnClient.code.message;

namespace QnClient.code.hud.system;

public partial class System : NinePatchRect, ICharacterJoinedAware
{
    private TextureButton _systemSetting;
    private TextureButton _exit;
    
    private TextureButton _return;

    private Setting _setting;
    
    public override void _Ready()
    {
        _return = GetNode<TextureButton>("Return");
        _return.Pressed += () => Visible = false;
        _systemSetting = GetNode<TextureButton>("SystemSetting");
        _exit = GetNode<TextureButton>("Exit");
        _exit.Pressed += () => GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
        _setting = GetNode<Setting>("Setting");
        _systemSetting.Pressed += () => _setting.Visible = true;
        _setting.ClosePressed += () =>
        {
            _setting.Visible = false;
            Visible = false;
        };
        Visible = false;
    }

    public void SetSettingChangedListener(Action<ISystemSetting> listener)
    {
        _setting.SettingChanged += listener;
    }

    public void OnButtonPressed()
    {
        Visible = true;
    }

    public void OnCharacterJoined(JoinRealmMessage message)
    {
        _setting.OnCharacterJoined();
    }
}