using Godot;
using QnClient.code.ui;

namespace QnClient.code.debug;

public partial class DebugTextBubble : Node2D
{
    private Sprite2D _sprite;
    private TextBubble _textBubble;
    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite2D");
        _textBubble = GetNode<TextBubble>("TextBubble");
    }
    

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventKey eventKey || eventKey.Pressed)
        {
            return;
        }
        switch (eventKey.Keycode)
        {
            case Key.Key1:
                _textBubble.Display("雷剑", _sprite.Texture.GetSize());
                break;
            case Key.Key2:
                _textBubble.Display("回转狂天飞", _sprite.Texture.GetSize());
                break;
        }
    }
}