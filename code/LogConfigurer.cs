using Godot;
using NLog;
using NLog.Layouts;

namespace QnClient.code;


public partial class LogConfigurer: Node
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        LogManager.Setup().LoadConfiguration(builder => {
            builder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteToConsole(Layout.FromString("${date} | ${threadid} | ${level:uppercase=true} | ${logger} | ${message} ${exception}"));
        });
    }
}