namespace QnClient.code.player.character;

public readonly struct ValueBar(int current, int max)
{
    public int Current { get; } = current;

    public int Max { get; } = max;

    public int Percent => Current * 100 / Max;

    public string Text => ((float)Current / 100).ToString("0.00") + "/" + ((float)Max / 100).ToString("0.00");

    public static readonly ValueBar Default = new(1, 1);
}