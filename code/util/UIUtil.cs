using Godot;

namespace QnClient.code.util;

public static class UiUtil
{

    public static Vector2 GetTextSize(this Control control, string text)
    {
        var font = control.GetThemeFont("normal_font");
        return font.GetStringSize(text, HorizontalAlignment.Left, -1, 12);
    }
}