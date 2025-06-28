using System;
using System.Linq;
using Godot;

namespace QnClient.code.ui;

public abstract partial class AbstractSlotContainer : NinePatchRect
{
    private Slot[] _slots = new Slot[30];

    public override void _Ready()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i] = CreateSlot("Slot" + (i + 1));
            _slots[i].LeftMouseButtonReleased += OnSlotLeftMouseButtonReleased;
            _slots[i].LeftMouseButtonDoubleClicked += OnSlotLeftButtonDoubleClicked;
            _slots[i].RightMouseButtonReleased += OnSlotLeftButtonDoubleClicked;
            GetNode<GridContainer>("GridContainer").AddChild(_slots[i]);
            Visible = false;
        }
        GetNode<Button>("CloseButton").Pressed += () => Visible = false;
    }

    protected Slot? FindSlotHasHovering()
    {
        return _slots.FirstOrDefault(s => s.MouseHovering);
    }

    protected void ForeachSlot(Action<Slot> action)
    {
        foreach (var slot in _slots)
        {
            action.Invoke(slot);
        }
    }

    protected Slot GetSlot(int slotNumber)
    {
        return _slots[slotNumber - 1];
    }

    protected abstract Slot CreateSlot(string name);

    protected abstract void OnSlotLeftMouseButtonReleased(int number);
    
    protected abstract void OnSlotLeftButtonDoubleClicked(int number);
    
    protected abstract void OnSlotRightMouseButtonReleased(int number);
}