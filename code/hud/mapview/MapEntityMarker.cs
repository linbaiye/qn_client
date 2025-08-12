using Godot;

namespace QnClient.code.hud.mapview;

public partial class MapEntityMarker : Panel
{
    private Label _label;

    private string _name = "";

    private long _id = 0;

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

    public void SetEntityIdName(long id, string name)
    {
        _name = name;
        _id = id;
    }
    
    public long Id => _id;
    

    private void OnMouseExited()
    {
        _label.Text = "";
        _label.Hide();
    }
    

    public static MapEntityMarker Create(string name, long id = 0)
    {
        var packedScene = ResourceLoader.Load<PackedScene>("res://scene/ui/map/map_entity_marker.tscn");
        var view = packedScene.Instantiate<MapEntityMarker>();
        view._name = name;
        view._id = id;
        return view;
    }
}