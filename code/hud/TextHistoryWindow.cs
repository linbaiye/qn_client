using System;
using System.Collections.Generic;
using Godot;

namespace QnClient.code.hud;

public partial class TextHistoryWindow : ScrollContainer
{
    private VBoxContainer _container;

    private readonly List<RichTextLabel> _labels = new();

     private Timer _timer;

    private bool _userScrolledToLast = true;
    
    public override void _Ready()
    {
        _container = GetNode<VBoxContainer>("VBoxContainer");
        Visible = false;
        _timer = GetNode<Timer>("Timer");
        _timer.Timeout += HandleTimeout;
    }

    private void OnScrollEnded()
    {
        if (Math.Abs(GetVScrollBar().MaxValue - GetVScroll()) > 10)
            _userScrolledToLast = false;
    }

    private bool IsAtBottom()
    {
        if (GetVScrollBar().MaxValue < GetSize().Y)
            return true;
        var diff = GetVScrollBar().MaxValue - (GetVScrollBar().Value + GetSize().Y);
        return Math.Abs(diff) <= 16;
    }

    private void HandleTimeout()
    {
        if (!Visible)
            return;
        if (IsAtBottom())
            ScrollVertical = (int)GetVScrollBar().MaxValue;
    }

    private void DoClear()
    {
        _labels.Clear();
        foreach (var child in _container.GetChildren())
        {
            child.QueueFree();
        }
    }

    public void Display(List<RichTextLabel> labels)
    {
        DoClear();
        foreach (var richTextLabel in labels)
        {
            _container.AddChild(richTextLabel);
            _labels.Add(richTextLabel);
        }
        Visible = true;
        _timer.Start(0.5f);
    }

    public void Append(List<RichTextLabel> labels)
    {
        if (!Visible)
            return;
        foreach (var label in labels)
        {
            if (_labels.Count >= 500)
            {
                var richTextLabel = _labels[0];
                _container.RemoveChild(richTextLabel);
                _labels.RemoveAt(0);
            }
            _container.AddChild(label);
        }
    }

    public void ClearAndHide()
    {
        DoClear();
        Visible = false;
        _timer.Stop();
    }
    
}