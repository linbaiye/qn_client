using System.Collections.Generic;
using System.Text.Json;
using Godot;
using QnClient.code.hud.bottom;
using QnClient.code.input;
using QnClient.code.network;
using QnClient.code.util;

namespace QnClient.code.hud.assistance;

public partial class HealAssistance : NinePatchRect, IConnectionAware
{
    
    private PillSetting[] _pillSettings = new PillSetting[7];

    private Connection _connection;

    private int _requestSlot = 0;

    private IAttributeProvider _attributeProvider;

    private Timer _timer;

    private const int InnerPower = 2;
    private const int OuterPower = 3;
    private const int Power = 4;
    private const int Life = 5;
    private const int LowLife = 6;
    
    private LineEdit _interval;
    
    private CheckBox _expMode;

    private bool _used;

    private Label _errorTip;

    private int _expIndex;
    
    private FileStorage _fileStorage;

    public override void _Ready()
    {
        for (int i = 1; i <= 6; i++)
        {
            _pillSettings[i] = GetNode<PillSetting>("PillSetting" + i);
            _pillSettings[i].DropdownPressed += RequestPills;
        }
        _timer = GetNode<Timer>("Timer");
        _timer.Timeout += CheckAndFill;
        _interval = GetNode<LineEdit>("Interval");
        _interval.Text = "300";
        _interval.TextChanged += OnIntervalChanged;
        _expMode = GetNode<CheckBox>("ExpMode");
        _errorTip = GetNode<Label>("ErrorTip");
        Visible = false;
        _expIndex = -1;
    }


    private bool IntervalValid()
    {
        if (!_interval.DigitOnly())
        {
            return false;
        }
        var v = int.Parse(_interval.Text);
        if (v < 100 || v > 9999)
        {
            return false;
        }
        return true;
    }

    private void OnIntervalChanged(string text)
    {
        _errorTip.Text = null;
        if (!IntervalValid())
        {
            _errorTip.Text = "延迟只能在100-9999之间";
            return;
        }
        var v = int.Parse(_interval.Text);
        _timer.Stop();
        _timer.Start((double)v / 1000);
    }

    public void Popup()
    {
        _errorTip.Text = null;
        Visible = true;
    }
    
    
    private static readonly Dictionary<int, AttributeType> IndexTypeMap = new()
    {
        { LowLife, AttributeType.Life },
        { InnerPower, AttributeType.InnerPower },
        { OuterPower, AttributeType.OutPower },
        { Power, AttributeType.Power },
        { Life, AttributeType.Life },
    };

    private void CheckAndFillExp(int index)
    {
        if (_used || !IndexTypeMap.TryGetValue(index, out var type))
            return;
        var pillSetting = _pillSettings[index];
        if (!pillSetting.Active)
            return;
        if (_expIndex == index)
        {
            if (_attributeProvider.GetPercent(type) < 90)
            {
                _used = true;
                _connection.WriteAndFlush(new UsePillInput(pillSetting.PillName));
            }
            else
                _expIndex = -1;
            return;
        }
        if (_attributeProvider.GetPercent(type) >= pillSetting.Percent)
            return;
        _connection.WriteAndFlush(new UsePillInput(pillSetting.PillName));
        if (_expMode.ButtonPressed)
            _expIndex = index;
        _used = true;
    }

    private void CheckAndFillNormal(int index)
    {
        if (_used || !IndexTypeMap.TryGetValue(index, out var type))
            return;
        var pillSetting = _pillSettings[index];
        if (pillSetting.Active &&
            _attributeProvider.GetPercent(type) <= pillSetting.Percent)
        {
            _used = true;
            _connection.WriteAndFlush(new UsePillInput(pillSetting.PillName));
        }
    }


    private void CheckAndFill()
    {
        _used = false;
        CheckAndFillNormal(LowLife);
        if (_expIndex != -1)
            CheckAndFillExp(_expIndex);
        CheckAndFillNormal(Life);
        CheckAndFillExp(Power);
        CheckAndFillExp(InnerPower);
        CheckAndFillExp(OuterPower);
    }

    private void RequestPills(int n)
    {
        _requestSlot = n;
        _connection?.WriteAndFlush(SimpleInput.GetPills);
    }

    public void SetConnection(Connection connection)
    {
        _connection = connection;
    }

    public void SetAttributeProvider(IAttributeProvider attributeProvider)
    {
        _attributeProvider = attributeProvider;
    }

    public void OnCharacterJoined()
    {
        _fileStorage = new FileStorage("heal_assistance");
        var content = _fileStorage.ReadContent();
        if (string.IsNullOrEmpty(content))
        {
            _timer.Start(0.3f);
        }
        else
        {
            Restore(content);
        }
    }


    private void Restore(string content)
    {
        try
        {
            var obj = JsonSerializer.Deserialize<JsonObject>(content);
            _interval.Text = obj.Interval;
            _expMode.ButtonPressed = obj.ExpMode;
            for (int i = 1; i <= 6; i++)
            {
                _pillSettings[i].Deserialize(obj.PillSettings[i-1]);
            }
            int v = IntervalValid() ? int.Parse(_interval.Text.Trim()) : 300;
            _timer.Start((double) v / 1000);
        }
        catch
        {
            _fileStorage.Delete();
        }
    }
    
    
    private class JsonObject
    {
        public string[] PillSettings { get; set; }
        
        public bool ExpMode { get; set; }
        
        public string Interval { get; set; }
    }

    public void Save()
    {
        if (_fileStorage == null)
            return;
        string[] settings = new string[6];
        for (int i = 1; i <= 6; i++)
        {
            settings[i-1] = _pillSettings[i].Serialize;
        }
        JsonObject jsonObject = new JsonObject();
        jsonObject.Interval = _interval.Text;
        jsonObject.ExpMode = _expMode.ButtonPressed;
        jsonObject.PillSettings = settings;
        _fileStorage.Save(JsonSerializer.Serialize(jsonObject));
    }

    public void FillPills(List<string> pills)
    {
        if (_requestSlot > 0)
            _pillSettings[_requestSlot].FillItems(pills);
    }
}