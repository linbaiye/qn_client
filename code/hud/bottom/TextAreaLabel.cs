using System;
using Godot;
using QnClient.code.message;

namespace QnClient.code.hud.bottom;

public partial class TextAreaLabel : RichTextLabel
{
    private TextMessage.TextType _type;
    private const string ThemeName = "normal";

    public event Action<string>? PrivateChatPressed;
    private string _originText;

    public void Populate(string text, string color, string bgColor, TextMessage.TextType type = TextMessage.TextType.Normal)
    {
        if (!string.IsNullOrEmpty(color))
            Text = "[color=" + color + "]" + text + "[/color]";
        else
            Text = text;
        if (!string.IsNullOrEmpty(bgColor))
            AddThemeStyleboxOverride(ThemeName, new StyleBoxFlat() { BgColor = new Color(bgColor)});
        _originText = text;
        _type = type;
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is not InputEventMouse mouse || !mouse.IsPressed())
            return;
        AcceptEvent();
        if (_type != TextMessage.TextType.PrivateChat || string.IsNullOrEmpty(_originText))
            return;
        if (_originText.StartsWith(">"))
        {
            var chunks = _originText.Substring(1).Split("：");
            if (chunks.Length < 2)
                return;
            PrivateChatPressed?.Invoke(chunks[0]);
        }
        else
        {
            var chunks = _originText.Split(">");
            if (chunks.Length < 2)
                return;
            PrivateChatPressed?.Invoke(chunks[0]);
        }
    }
    

    public void Clean()
    {
        Text = null;
        _originText = null;
        RemoveStyleBox();
    }

    public void Copy(TextAreaLabel another)
    {
        _originText = another._originText;
        Text = another.Text;
        RemoveStyleBox();
        if (another.HasStyleBox)
            AddStyleBox(another.GetStyleBox);
        _type = another._type;
    }


    private void RemoveStyleBox()
    {
        if (HasStyleBox)
            RemoveThemeStyleboxOverride(ThemeName);
    }

    private void AddStyleBox(StyleBox styleBox)
    {
        AddThemeStyleboxOverride(ThemeName, styleBox);
    }

    private bool HasStyleBox => HasThemeStyleboxOverride(ThemeName);

    private StyleBox GetStyleBox => HasStyleBox ? GetThemeStylebox(ThemeName) : null;
}