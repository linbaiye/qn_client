using System.Collections.Generic;
using System.Text;
using Godot;

namespace QnClient.code.ui;

public partial class TextBubble : RichTextLabel
{
    private const int MaxLines = 4;
    private const int MaxLineLength = 20;

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
    
    
    public void Display(string text, Vector2 textureSize)
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
        var font = GetThemeFont("normal_font");
        var size = font.GetStringSize(lines[0]);
        if (lines.Count > 1)
        {
            size *= new Vector2(1, lines.Count);
            size += new Vector2(16, 6);
        }
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string line in lines)
        {
            stringBuilder.Append(line);
        }
        Text = stringBuilder.ToString();
        Size = size;
        Position = new Vector2(textureSize.X / 2 - Size.X / 2, - 60);
    }

    public void Display(string text)
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
        var font = GetThemeFont("normal_font");
        var size = font.GetStringSize(lines[0]);
        if (lines.Count > 1)
        {
            size *= new Vector2(1, lines.Count);
            size += new Vector2(16, 6);
        }
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string line in lines)
        {
            stringBuilder.Append(line);
        }
        Text = stringBuilder.ToString();
        Size = size;
    }

}