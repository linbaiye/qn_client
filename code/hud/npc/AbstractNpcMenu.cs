using Godot;
using QnClient.code.network;
using QnClient.code.network.toserver;
using QnClient.code.sprite;

namespace QnClient.code.hud.npc;

public abstract partial class AbstractNpcMenu : NinePatchRect, IConnectionAware
{
    private Label _name;
    private Label _greetings;
    private TextureRect _caption;
    private Button _closeButton;
    private long _npcId;

    private Connection _connection;

    public override void _Ready()
    {
        _name = GetNode<Label>("Name");
        _greetings = GetNode<Label>("Greetings");
        _caption = GetNode<TextureRect>("Caption");
        _closeButton = GetNode<Button>("CloseButton");
        _closeButton.Pressed += OnClose;
        Visible = false;
    }
    
    protected long NpcId => _npcId;

    protected void SetFields(string name, long npcId, string greetings, string sprite, int index)
    {
        _name.Text = name;
        _npcId = npcId;
        _greetings.Text = greetings;
        var textures = ZipFileSpriteLoader.Instance.Load(sprite);
        _caption.Texture = textures[index].Texture;
    }

    protected abstract void OnClose();

    public void SetConnection(Connection connection)
    {
        _connection = connection;
    }

    protected void SendMessage(I2ServerMessage message)
    {
        _connection.WriteAndFlush(message);
    }
}