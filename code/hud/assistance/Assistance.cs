using System;
using System.Collections.Generic;
using Godot;
using QnClient.code.entity;
using QnClient.code.hud.bottom;
using QnClient.code.message;
using QnClient.code.network;

namespace QnClient.code.hud.assistance;

public partial class Assistance : NinePatchRect, IConnectionAware, ICharacterJoinedAware
{
    private Tab _healTab;
    private Tab _lootTab;
    
    private Button _closeButton;
    
    private HealAssistance _healAssistance;
    
    private LootAssistance _lootAssistance;

    public override void _Ready()
    {
        _healTab = GetNode<Tab>("HealTab");
        _healTab.SetTextures(ResourceLoader.Load<Texture2D>("res://ui/hud/assistance/heal.png"), ResourceLoader.Load<Texture2D>("res://ui/hud/assistance/heal_focus.png"));
        _healTab.Pressed += HealTabPressed;
        _healTab.GainFocus();
        _closeButton = GetNode<Button>("Close");
        _closeButton.Pressed += () => Visible = false;
        _healAssistance = GetNode<HealAssistance>("HealAssistance");
        
        _lootTab = GetNode<Tab>("LootTab");
        _lootTab.SetTextures(ResourceLoader.Load<Texture2D>("res://ui/hud/assistance/loot.png"), ResourceLoader.Load<Texture2D>("res://ui/hud/assistance/loot_focus.png"));
        _lootTab.Pressed += LootTabPressed;
        _lootAssistance = GetNode<LootAssistance>("LootAssistance");
        Visible = false;
    }
    private void LootTabPressed()
    {
        _healTab.LoseFocus();
        _healAssistance.Hide();
        _lootTab.GainFocus();
        _lootAssistance.Popup();
    }
    
    private void HealTabPressed()
    {
        _lootTab.LoseFocus();
        _lootAssistance.Hide();
        _healTab.GainFocus();
        _healAssistance.Popup();
    }

    public void ButtonPressed()
    {
        if (Visible)
        {
            Visible = false;
            return;
        }
        if (_healTab.IsFocused)
            HealTabPressed();
        else
            LootTabPressed();
        Visible = true;
    }

    public void SetConnection(Connection connection)
    {
        _healAssistance.SetConnection(connection);
        _lootAssistance.SetConnection(connection);
    }

    public void SetAttributeProvider(IAttributeProvider attributeProvider)
    {
        _healAssistance.SetAttributeProvider(attributeProvider);
    }

    public void FillPills(List<string> pills)
    {
        _healAssistance.FillPills(pills);
    }
    
    
    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            _healAssistance.Save();
            _lootAssistance.Save();
        }
    }
    
    public void SetItemFilter(Func<IEnumerable<GroundItem>> action)
    {
        _lootAssistance.SetItemFilter(action);
    }

    public void OnCharacterCoordinateChanged(Vector2I coor)
    {
        _lootAssistance.OnCharacterCoordinateChanged(coor);
    }

    public void OnCharacterJoined(JoinRealmMessage message)
    {
        _healAssistance.OnCharacterJoined();
        _lootAssistance.OnCharacterJoined(message.Coordinate);
    }
}