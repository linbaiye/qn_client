namespace QnClient.code.player.character.kungfu;

public class FootKungFu(string name, bool fly) : KungFu(name)
{
    public bool CanFly { get; } = fly;
}