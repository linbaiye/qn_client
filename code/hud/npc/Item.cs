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
    
    private Label _lockedReason;
    
    private bool _locked;

    private int _iconColor;

    private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

    public override void _Ready()
    {
        _detailsContainer = GetNode<VFlowContainer>("DetailsContainer");
        _iconContainer = _detailsContainer.GetNode<CenterContainer>("IconContainer");
        _name = _detailsContainer.GetNode<Label>("Name");
        _price = _detailsContainer.GetNode<Label>("Price");
        _lockedReason = _detailsContainer.GetNode<Label>("LockedReason");
    }
    
    public string ItemName => _name.Text;

    public int Price => int.Parse(_price.Text);

    public int IconColor => _iconColor;

    public void ToggleHighlight(bool highlight)
    {
        if (_locked)
        {
            
            return;
        }
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
                Log.Debug("DoubleClick");
                //Clicked?.Invoke(this, new SlotMouseEvent(SlotMouseEvent.Type.MOUSE_LEFT_DOUBLE_CLICK));
            else
                Log.Debug("click");
                //Clicked?.Invoke(this, new SlotMouseEvent(SlotMouseEvent.Type.MOUSE_LEFT_CLICK));
        }
    }

    public void SetDetails(string name, Texture2D icon, int iconColor, int price)
    {
        if (_locked)
        {
            return;
        }
        _name.Text = name;
        _price.Text = price.ToString();
        var textureRect = _iconContainer.GetNode<TextureRect>("Icon");
        textureRect.Texture = icon;
        if (iconColor != 0)
        {
            textureRect.Material = DyeShader.CreateShaderMaterial(iconColor);
        }
        _iconColor = iconColor;
    }

    public void Lock(string text = "")
    {
        if (_locked) 
            return;
        _locked = true;
        RemoveThemeStyleboxOverride("panel");
        _lockedReason.Text = text;
        AddThemeStyleboxOverride("panel", new StyleBoxFlat()
        {
            BgColor = new Color("4a4a4a")
        });
    }

    public void UpdateText(string text) 
    {
        RemoveThemeStyleboxOverride("panel");
        _lockedReason.Text = text;
        AddThemeStyleboxOverride("panel", new StyleBoxFlat()
        {
            BgColor = new Color("4a4a4a")
        });
    }
    
}