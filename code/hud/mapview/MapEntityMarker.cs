using Godot;

namespace QnClient.code.hud.mapview;

public partial class MapEntityMarker : Panel
{
    private Label _label;

    private string _name = "";

    public override void _Ready()
    {
        _label = GetNode<Label>("Name");
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }
    
    private void OnMouseEntered()
    {
        _label.Text = _name;
        _label.Position = new Vector2(10, 0);
        _label.Show();
    }

    public void SetEntityName(string name)
    {
        _name = name;
    }
    
    public string EntityName => _name;

    private void OnMouseExited()
    {
        _label.Text = "";
        _label.Hide();
    }


    public static MapEntityMarker Create(string name)
    {
        var packedScene = ResourceLoader.Load<PackedScene>("res://scene/ui/map/map_entity_marker.tscn");
        var view = packedScene.Instantiate<MapEntityMarker>();
        view._name = name;
        return view;
    }
}