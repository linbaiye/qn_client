using Godot;
using QnClient.code.sprite;

namespace QnClient.code.hud.attribute;

public partial class Equipment : NinePatchRect
{

    private VBoxContainer _left;
    
    private VBoxContainer _right;

    private Texture2D[] _itemIcons;
    public override void _Ready()
    {
        _itemIcons = ZipFileSpriteLoader.Instance.LoadOrderedItemIcons();
        _left = GetNode<VBoxContainer>("Left");
        _right = GetNode<VBoxContainer>("Right");
        for (int i = 1; i <= 8; i++)
        {
            var slot = Slot.Create("Slot", new Vector2(34, 34), new Vector2(34, 34), false);
            if (i <= 4)
                _left.AddChild(slot);
            else
                _right.AddChild(slot);
            slot.SetDetails(_itemIcons[1], "test");
        }
    }
}