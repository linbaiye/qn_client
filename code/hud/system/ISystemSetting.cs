namespace QnClient.code.hud.system;

public interface ISystemSetting
{
    
    public bool BgmEnabled { get; }
    public bool SoundEnabled { get; }
    public double BgmVolume { get; }
    public double SoundVolume { get; }
    
}