using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using NLog;

namespace QnClient.code.hud.bottom;

public partial class TextArea : VBoxContainer
{
    private const int MaxLines = 5;
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

    private RichTextLabel[] _lines = new RichTextLabel[MaxLines];
    
    private const string ThemeName = "normal";
    public override void _Ready()
    {
        for (int i = 0; i < _lines.Length; i++)
            _lines[i] = GetNode<RichTextLabel>("Line" + (i + 1));
    }

    private void MakeBlankLine()
    {
        for (int i = 0; i < _lines.Length - 1; i++)
        {
            _lines[i].Text = _lines[i + 1].Text;
        }
        _lines[MaxLines - 1].Text = null;
    }

    private bool IsAllLinesOccupied()
    {
        return !_lines.Any(l => string.IsNullOrEmpty(l.Text));
    }

    public void Display(string text)
    {
        var lines = SplitByNewline(text);
        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }
            if (IsAllLinesOccupied())
            {
                MakeBlankLine();
            }
            foreach (var richTextLabel in _lines)
            {
                if (string.IsNullOrEmpty(richTextLabel.Text))
                {
                    richTextLabel.Text = line;
                }
            }
        }
    }
    
    private static List<string> SplitByNewline(string str)
    {
        return [..str.Split(["\r\n", "\r", "\n"], StringSplitOptions.None)];
    }
}