using System;
using Godot;
using NLog;
using QnClient.code.entity;

namespace QnClient.code.hud.npc;

public partial class Item : Panel
{
    private VFlowContainer _detailsContainer;

    private CenterContainer _iconContainer;
    
    private Label _name;
    
    private Label _price;
    
    private Label _costLabel;

    private int _cost;
    
    private bool _locked;

    private int _iconColor;

    private bool _canStack;

    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

    public event Action<Item>? Clicked;
    public event Action<Item>? DoubleClicked;

    public override void _Ready()
    {
        _detailsContainer = GetNode<VFlowContainer>("DetailsContainer");
        _iconContainer = _detailsContainer.GetNode<CenterContainer>("IconContainer");
        _name = _detailsContainer.GetNode<Label>("Name");
        _price = _detailsContainer.GetNode<Label>("Price");
        _costLabel = _detailsContainer.GetNode<Label>("Cost");
    }
    
    public string ItemName => _name.Text;

    public int Price => int.Parse(_price.Text);

    public int IconColor => _iconColor;

    public int Cost => _cost;

    public bool CanStack => _canStack;
    
    public int Icon { get; private set; }

    public void ToggleHighlight(bool highlight)
    {
        if (highlight)
        {
            AddThemeStyleboxOverride("panel", new StyleBoxFlat()
            {
                BgColor = new Color("787878")
            });
        }
        else
        {
            RemoveThemeStyleboxOverride("panel");
        }
    }

    public override void _GuiInput(InputEvent inputEvent)
    {
        if (inputEvent.IsPressed() &&
            inputEvent is InputEventMouseButton mouseButton &&
            mouseButton.ButtonIndex == MouseButton.Left)
        {
            GetViewport().SetInputAsHandled();
            if (_locked)
            {
                return;
            }
            if (mouseButton.DoubleClick)
                DoubleClicked?.Invoke(this);
            else
                Clicked?.Invoke(this);
        }
    }

    public void SetDetails(string name, Texture2D iconTexture, int iconColor, int price, bool canstack, int icon)
    {
        _name.Text = name;
        _price.Text = price.ToString();
        var textureRect = _iconContainer.GetNode<TextureRect>("Icon");
        textureRect.Texture = iconTexture;
        if (iconColor != 0)
        {
            textureRect.Material = DyeShader.CreateShaderMaterial(iconColor);
        }
        _iconColor = iconColor;
        _canStack = canstack;
        Icon = icon;
    }

    public void Lock(string text = "")
    {
        RemoveThemeStyleboxOverride("panel");
        _costLabel.Text = text;
        AddThemeStyleboxOverride("panel", new StyleBoxFlat()
        {
            BgColor = new Color("4a4a4a")
        });
    }

    public void AddCost(int cost) 
    {
        _cost += cost;
        _costLabel.Text = _cost.ToString();
        // if (GetThemeStylebox("panel") == null)
        //     AddThemeStyleboxOverride("panel", new StyleBoxFlat()
        //     {
        //         BgColor = new Color("4a4a4a")
        //     });
    }

    public static Item Create()
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>("res://scene/ui/npc/item.tscn");
        return scene.Instantiate<Item>();
    }
    
}