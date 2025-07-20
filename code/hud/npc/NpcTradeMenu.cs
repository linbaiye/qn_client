namespace QnClient.code.hud.npc;

public partial class NpcTradeMenu : AbstractNpcMenu
{
    private ScrollItemContainer _itemContainer;
    public override void _Ready()
    {
        base._Ready();
        _itemContainer = GetNode<ScrollItemContainer>("ScrollItemContainer");
    }
    
    public void 
}