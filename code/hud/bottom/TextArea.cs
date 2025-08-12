using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using NLog;
using QnClient.code.message;

namespace QnClient.code.hud.bottom;

public partial class TextArea : VBoxContainer
{
    private const int MaxLines = 5;
    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

    private TextAreaLabel[] _lines = new TextAreaLabel[MaxLines];
    
    private readonly MessageHistory _privateChat = new(100);
    
    private readonly MessageHistory _allMessage = new(200);

    private OptionButton _chatOption;
    
    private TextHistoryWindow _textHistoryWindow;

    private Button _textHistoryButton;
    
    
    public override void _Ready()
    {
        for (int i = 0; i < _lines.Length; i++)
        {
            _lines[i] = GetNode<TextAreaLabel>("Line" + (i + 1));
            _lines[i].PrivateChatPressed += PrivateChatPressed;
        }
        _chatOption = GetNode<OptionButton>("../ChatOption");
        _chatOption.ItemSelected += OnChatOptionChanged;
        _textHistoryButton = GetNode<Button>("../TextHistoryButton");
        _textHistoryButton.Pressed += OnHistoryButtonPressed;
    }

    private bool IsPrivateChatSelected => _chatOption.Selected == 1;
    private bool IsNormalSelected => _chatOption.Selected == 0;

    private void PrivateChatPressed(string name)
    {
        GD.Print(name);
    }

    private void OnHistoryButtonPressed()
    {
        if (_textHistoryWindow.Visible)
        {
            _textHistoryWindow.ClearAndHide();
        }
        else
        {
            var msgs = IsPrivateChatSelected ? _privateChat.GetAll : _allMessage.GetAll;
            _textHistoryWindow.Display(BuildHistoryWindowLabels(msgs));
        }
    }

    private void MakeBlankLine()
    {
        for (int i = 0; i < _lines.Length - 1; i++)
        {
            _lines[i].Copy(_lines[i + 1]);
        }
        _lines[MaxLines - 1].Clean();
    }

    private bool IsAllLinesOccupied()
    {
        return !_lines.Any(l => string.IsNullOrEmpty(l.Text));
    }


    private void OnChatOptionChanged(long id)
    {
        Clear();
        var last5msg = IsPrivateChatSelected ? _privateChat.Last5Messages() : _allMessage.Last5Messages();
        foreach (var msg in last5msg)
        {
            Display(msg.Text, msg.Color, msg.BgColor, msg.Type);
        }

        if (_textHistoryWindow.Visible)
        {
            _textHistoryWindow.Display(BuildHistoryWindowLabels(IsPrivateChatSelected ? _privateChat.GetAll : _allMessage.GetAll));
        }
    }

    private void Clear()
    {
        foreach (var line in _lines)
            line.Clean();
    }

    private List<TextAreaLabel> BuildHistoryWindowLabels(List<TextMessage> messages)
    {
        List<TextAreaLabel> result = new List<TextAreaLabel>();
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://scene/ui/bottom/text_area_label.tscn");
        foreach (var textMessage in messages)
        {
            var lines = SplitByNewline(textMessage.Text);
            foreach (var line in lines)
            {
                var richTextLabel = scene.Instantiate<TextAreaLabel>();
                richTextLabel.PrivateChatPressed += PrivateChatPressed;
                richTextLabel.Populate(line, textMessage.Color, textMessage.BgColor, textMessage.Type);
                result.Add(richTextLabel);
            }
        }
        return result;
    }

    public void SetTextHistoryWindow(TextHistoryWindow window)
    {
        _textHistoryWindow = window;
    }

    public void Display(TextMessage message)
    {
        _allMessage.Add(message);
        if (message.Type == TextMessage.TextType.PrivateChat)
        {
            _privateChat.Add(message);
            if (IsPrivateChatSelected)
                _textHistoryWindow.Append(BuildHistoryWindowLabels([message]));
            Display(message.Text, message.Color, message.BgColor, message.Type);
        }
        else
        {
            if (IsNormalSelected)
            {
                Display(message.Text, message.Color, message.BgColor, message.Type);
                _textHistoryWindow.Append(BuildHistoryWindowLabels([message]));
            }
        }
    }

    public void Display(string text, string color, string bgColor, TextMessage.TextType type = TextMessage.TextType.Normal)
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
                    richTextLabel.Populate(line, color, bgColor, type);
                    break;
                }
            }
        }
    }
    
    private static List<string> SplitByNewline(string str)
    {
        return [..str.Split(["\r\n", "\r", "\n"], StringSplitOptions.None)];
    }
}