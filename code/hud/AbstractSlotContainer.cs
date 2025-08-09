using System;
using System.Linq;
using Godot;

namespace QnClient.code.hud;

public abstract partial class AbstractSlotContainer : NinePatchRect
{
    private Slot[] _slots;
    
    public event Action<int, InputEventKey>? KeyPressedOnSlot;

    public override void _Ready()
    {
        _slots = new Slot[Capacity];
        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i] = CreateSlot("Slot" + (i + 1));
            _slots[i].LeftMouseButtonReleased += OnLeftButtonReleased;
            _slots[i].LeftMouseButtonDoubleClicked += OnSlotLeftButtonDoubleClicked;
            _slots[i].RightMouseButtonReleased += OnSlotRightMouseButtonReleased;
            _slots[i].KeyPressed += OnKeyPressed;
            GetNode<GridContainer>("GridContainer").AddChild(_slots[i]);
        }
        Visible = false;
        GetNode<Button>("CloseButton").Pressed += OnCloseButtonClicked;
    }

    protected virtual void OnCloseButtonClicked()
    {
        Visible = false;
    }


    private void OnLeftButtonReleased(int draggedSlot)
    {
        var slot = FindSlotHasHovering();
        if (slot != null && slot.SlotNumber == draggedSlot)
        {
            return;
        }
        if (!GetSlot(draggedSlot).Empty)
            OnNonEmptyDragReleased(draggedSlot);
    }


    public Slot? FindSlotHasHovering()
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

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventMouse)
        {
            GetViewport().SetInputAsHandled();
        }
    }

    private void OnKeyPressed(int number, InputEventKey key)
    {
        KeyPressedOnSlot?.Invoke(number, key);
    }

    public Slot GetSlot(int slotNumber)
    {
        return _slots[slotNumber - 1];
    }

    protected abstract Slot CreateSlot(string name);

    protected abstract void OnNonEmptyDragReleased(int number);
    
    protected abstract void OnSlotLeftButtonDoubleClicked(int number);
    
    protected abstract void OnSlotRightMouseButtonReleased(int number);
    
    protected abstract int Capacity { get; }
        
    public void ShowDescription(int slot, string text)
    {
        GetSlot(slot).ShowAttributeTipIfHasHover(text);
    }
}