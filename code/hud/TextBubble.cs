using System;
using System.Collections.Generic;
using System.Text;
using Godot;
using QnClient.code.util;

namespace QnClient.code.hud;

public partial class TextBubble : RichTextLabel
{
    private const int MaxLines = 4;
    private const int MaxLineLength = 20;
    
    private Timer _timer;

    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        _timer.OneShot = true;
        _timer.Timeout += () => Visible = false;
        Visible = false;
    }

    private List<string> BreakText(string text)
    {
        List<string> lines = new List<string>();
        if (text.Length <= MaxLineLength)
        {
            lines.Add(text);
            return lines;
        }
        int copiedNumber = 0;
        var stringBuilder = new StringBuilder();
        for (int i = 0; i < text.Length; i++) {
            stringBuilder.Append(text[i]);
            copiedNumber++;
            if (copiedNumber > MaxLineLength)
            {
                copiedNumber = 0;
                lines.Add(stringBuilder.ToString());
                stringBuilder = new StringBuilder();
            }
        }
        return lines;
    }
    
    
    public void Display(string text, Vector2 xCenterY)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }
        List<string> lines = BreakText(text);
        if (lines.Count > MaxLines)
        {
            lines = lines[..MaxLines];
        }
        Vector2 size = Vector2.Zero;
        foreach (var line in lines)
        {
            var tmp = this.GetTextSize(line);
            if (size.X < tmp.X + 20)
            {
                size = new Vector2(tmp.X + 20, size.Y);
            }
            if (size.Y < tmp.Y)
            {
                size = new Vector2(size.X, tmp.Y);
            }
        }
        size = new Vector2(size.X, size.Y * lines.Count + 10);
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string line in lines)
        {
            stringBuilder.Append(line);
        }
        Text = stringBuilder.ToString();
        Size = size;
        Position = new Vector2(xCenterY.X - Size.X / 2,  -50 - size.Y);
        _timer.Stop();
        Visible = true;
        _timer.Start(Double.Max(2f, Text.Length * 0.1f));
    }

}