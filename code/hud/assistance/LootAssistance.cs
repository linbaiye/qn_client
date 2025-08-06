using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Godot;
using QnClient.code.entity;
using QnClient.code.input;
using QnClient.code.network;

namespace QnClient.code.hud.assistance;

public partial class LootAssistance : NinePatchRect
{
    private ItemList _selected;
    
    private ItemList _shown;

    private Button _add;

    private LineEdit _itemName;

    private CheckBox _pick;
    
    private CheckBox _inversed;
    
    private Timer _timer;

    private Func<IEnumerable<GroundItem>>? _itemFilter;
    
    private Vector2I _characterCoordinate = Vector2I.Zero;

    private Connection _connection;

    private FileStorage _file;
    
    public override void _Ready()
    {
        _selected = GetNode<ItemList>("Selected");
        _selected.ItemClicked += OnSelectedItemClicked;
        
        _shown = GetNode<ItemList>("Shown");
        _shown.ItemClicked += OnShownItemClicked;
        
        _add = GetNode<Button>("Add");
        _add.Pressed += OnAddPressed;
        
        _itemName = GetNode<LineEdit>("ItemName");
        
        _pick = GetNode<CheckBox>("Pick");
        
        _inversed = GetNode<CheckBox>("Inversed");

        _timer = GetNode<Timer>("Timer");
        _timer.Timeout += OnTimeout;
        
        Visible = false;
    }

    private void OnTimeout()
    {
        var items = _itemFilter?.Invoke();
        if (items == null)
            return;
        var list = items.ToList();
        HandlePick(list);
        HandleShown(list);
    }

    private bool CanPick(Vector2I coor1, Vector2I coor2)
    {
        int xDist = Math.Abs(coor1.X - coor2.X);
        int yDist = Math.Abs(coor1.Y - coor2.Y);
        return xDist <= 3 && yDist <= 2;
    }

    private bool SelectedContains(string name)
    {
        var itemCount = _selected.GetItemCount();
        for (int i = 0; i < itemCount; i++)
        {
            if (_selected.GetItemText(i).Equals(name))
                return true;
        }
        return false;
    }
    
    private void HandlePick(IEnumerable<GroundItem> items)
    {
        if (!_pick.ButtonPressed)
            return;
        foreach (var groundItem in items)
        {
            if (!CanPick(groundItem.Coordinate, _characterCoordinate))
                continue;
            if (SelectedContains(groundItem.ItemName))
            {
                if (!_inversed.ButtonPressed)
                {
                    _connection.WriteAndFlush(new PickInput(groundItem.Id));
                    return;
                }
            }
            else 
            {
                if (_inversed.ButtonPressed)
                {
                    _connection.WriteAndFlush(new PickInput(groundItem.Id));
                    return;
                }
            }
        }
    }

    private void HandleShown(IEnumerable<GroundItem> items)
    {
 
        var stringNames = items.Select(i => i.ItemName);
        DisplayShown(stringNames);
    }

    private void OnAddPressed()
    {
        var name = _itemName.Text;
        if (!string.IsNullOrEmpty(name))
            AddToSelected(name.Trim());
    }

    private void AddToSelected(string name)
    {
        var itemCount = _selected.GetItemCount();
        for (int i = 0; i < itemCount; i++)
        {
            if (_selected.GetItemText(i).Equals(name))
                return;
        }
        _selected.AddItem(name);
        
    }
    
    private void OnSelectedItemClicked(long idx, Vector2 pos, long mouseButtonIndex)
    {
        if ((int)mouseButtonIndex != (int)MouseButton.Right)
        {
            return;
        }
        _selected.RemoveItem((int)idx);
    }

    private void OnShownItemClicked(long idx, Vector2 pos, long mouseButtonIndex)
    {
        if ((int)mouseButtonIndex != (int)MouseButton.Right)
        {
            return;
        }
        var ret = _shown.GetItemText((int)idx);
        AddToSelected(ret);
    }

    public void Popup()
    {
        Visible = true;
    }

    public void OnCharacterCoordinateChanged(Vector2I characterCoordinate)
    {
        _characterCoordinate = characterCoordinate;
    }

    public void SetConnection(Connection connection)
    {
        _connection = connection;
    }
    
    public void OnCharacterJoined(Vector2I characterCoordinate)
    {
        _characterCoordinate = characterCoordinate;
        _file = new FileStorage("loot_assistance");
        var content = _file.ReadContent();
        try
        {
            if (content != null)
            {
                var jsonObject = JsonSerializer.Deserialize<JsonObject>(content);
                jsonObject.Selected.ForEach(AddToSelected);
                _pick.ButtonPressed = jsonObject.Pick;
                _inversed.ButtonPressed = jsonObject.Inversed;
            }
        }
        catch
        {
            _file.Delete();
        }
        _timer.Start(0.1f);
    }

    public void SetItemFilter(Func<IEnumerable<GroundItem>> action)
    {
        _itemFilter = action;
    }

    private class JsonObject
    {
        public List<string> Selected { get; set; }
        public bool Pick { get; set; }
        public bool Inversed { get; set; }
    }

    private List<string> GetSelectedNames()
    {
        List<string> names = new List<string>();
        var itemCount = _selected.GetItemCount();
        for (int i = 0; i < itemCount; i++)
        {
            names.Add(_selected.GetItemText(i));
        }
        return names;
    }

    public void Save()
    {
        if (_file == null)
            return;
        JsonObject jsonObject = new JsonObject();
        jsonObject.Pick = _pick.ButtonPressed;
        jsonObject.Inversed = _inversed.ButtonPressed;
        jsonObject.Selected = GetSelectedNames();
        _file.Save(JsonSerializer.Serialize(jsonObject));
    }

    private void DisplayShown(IEnumerable<string> names)
    {
        _shown.Clear();
        var ordered = names.OrderBy(n => n);
        foreach (var name in ordered)
        {
            _shown.AddItem(name);
        }
    }
    
}