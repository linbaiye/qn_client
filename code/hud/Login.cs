using System;
using System.Collections.Generic;
using Godot;
using QnClient.code.account;
using QnClient.code.message;
using QnClient.code.network;

namespace QnClient.code.hud;

public partial class Login : NinePatchRect
{
    private LineEdit _username;
    private LineEdit _password;
    private TextureButton _connect;
    private TextureButton _regButton;
    private TextureButton _exit;
    private Label _label;
    private AudioStreamPlayer2D _buttonSound;

    private Connection _connection;

    public event Action? LoggedIn;
    
    public event Action? Exited;
    

    
    private NinePatchRect _register;
    private Label _regLabel;
    private LineEdit _regUsername;
    private LineEdit _regPassword;
    private LineEdit _regConfirmPassword;
    private Button _regConfirm;
    private Button _regReturn;
    private Timer _timer;
    
    
    
    private NinePatchRect _selectChar;
    private ItemList _charList;
    private Button _selectButton;
    private Button _createButton;
    private Button _deleteButton;
    private Label _selectLabel;
    private CheckBox _maleCheckBox;
    private CheckBox _femaleCheckBox;
    
    private LineEdit _selectInput;
    
    private static readonly bool DevMode = true;
    
    
    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
        _username = GetNode<LineEdit>("Username");
        _password = GetNode<LineEdit>("Password");
        _connect = GetNode<TextureButton>("Connect");
        _buttonSound = GetNode<AudioStreamPlayer2D>("ButtonSound");
        _exit = GetNode<TextureButton>("Exit");
        _exit.Pressed += () => Exited?.Invoke();
        _connect.Pressed += ConnectPressed;
        _regButton = GetNode<TextureButton>("RegButton");
        _regButton.Pressed += () =>
        {
            _buttonSound.Play();
            _register.Visible = true;
        };
        _register = GetNode<NinePatchRect>("Register");
        _register.Visible = false;
        _regLabel = _register.GetNode<Label>("Label");
        _regUsername = _register.GetNode<LineEdit>("Username");
        _regPassword = _register.GetNode<LineEdit>("Password");
        _regConfirmPassword = _register.GetNode<LineEdit>("ConfirmPassword");
        _regConfirm = _register.GetNode<Button>("Confirm");
        _regConfirm.Pressed += ConfirmRegister;
        _regReturn = _register.GetNode<Button>("Return");
        _regReturn.Pressed += () =>
        {
            _buttonSound.Play();
            _register.Visible = false;
        };
        _regButton.Disabled = true;
        _connect.Disabled = true;
        _timer = GetNode<Timer>("Timer");
        _timer.Timeout += HandleMessages;
        _selectChar = GetNode<NinePatchRect>("SelectChar");
        _charList = GetNode<ItemList>("SelectChar/CharList");
        _selectChar.Visible = false;
        _selectButton = GetNode<Button>("SelectChar/SelectButton");
        _createButton = GetNode<Button>("SelectChar/CreateButton");
        _deleteButton = GetNode<Button>("SelectChar/DeleteButton");
        _selectButton.Pressed += OnSelectCharClicked;
        _createButton.Pressed += OnCreateClicked;
        _deleteButton.Pressed += OnDeleteClicked;
        _selectLabel = GetNode<Label>("SelectChar/Label");
        _selectInput = GetNode<LineEdit>("SelectChar/Input");
        _maleCheckBox = GetNode<CheckBox>("SelectChar/MaleBox");
        _femaleCheckBox = GetNode<CheckBox>("SelectChar/FemaleBox");
        _maleCheckBox.SetPressedNoSignal(true);
        _maleCheckBox.Pressed += () => OnSexBoxChecked(_maleCheckBox);
        _femaleCheckBox.Pressed += () => OnSexBoxChecked(_femaleCheckBox);
    }
    

    private void ConfirmRegister()
    {
        _buttonSound.Play();
        var regUsernameText = _regUsername.Text;
        if (string.IsNullOrEmpty(regUsernameText))
        {
            _regLabel.Text = "请输入用户名";
            return;
        }

        if (string.IsNullOrEmpty(_regPassword.Text))
        {
            _regLabel.Text = "请输入密码";
            return;
        }
        if (string.IsNullOrEmpty(_regConfirmPassword.Text))
        {
            _regLabel.Text = "请输入密码确认";
            return;
        }
        if (!_regPassword.Text.Equals(_regConfirmPassword.Text))
        {
            _regLabel.Text = "密码不一致";
            return;
        }
        _connection?.WriteAndFlush(new RegisterAccountRequest(regUsernameText, _regPassword.Text));
    }


    private void HandleLoginAccountResponse(LoginAccountResponse loginAccountResponse)
    {
        if (loginAccountResponse.Code == 0)
        {
            AddToSelectChar(loginAccountResponse.Charnames);
        }
        else
        {
            _label.Text = loginAccountResponse.Msg;
        }
        
    }

    private void HandleMessages()
    {
        if (_connection == null)
            return;
        var messages = _connection.DrainMessages();
        foreach (object message in messages)
        {
            if (message is RegisterAccountResponse response)
            {
                _regLabel.Text = response.Msg;
            } 
            else if (message is LoginAccountResponse loginAccountResponse)
            {
                HandleLoginAccountResponse(loginAccountResponse);
            }
            else if (message is CreateCharacterResponse createCharacterResponse)
            {
                HandleCreateCharacterResponse(createCharacterResponse);
            }
        }
    }

    private void HandleCreateCharacterResponse(CreateCharacterResponse createCharacterResponse)
    {
        _selectLabel.Text = createCharacterResponse.Msg;
        if (createCharacterResponse.Code == 0)
        {
            _charList.AddItem(createCharacterResponse.CharacterName);
        }
    }

    private void AddToSelectChar(List<string> nameList)
    {
        foreach (var se in nameList)
        {
            int i = _charList.AddItem(se);
            _charList.SetItemTooltipEnabled(i, false);
        }
        if (nameList.Count > 0)
            _charList.Select(0);
        _selectChar.Visible = true;
    }

    private void OnCreateClicked()
    {
        _buttonSound.Play();
        if (string.IsNullOrEmpty(_selectInput.Text))
        {
            _selectLabel.Text = "请输入人物名字。";
            return;
        }
        bool male = _maleCheckBox.IsPressed();
        _connection.WriteAndFlush(new CreateCharacterRequest(_selectInput.Text, male));
    }

    private void OnDeleteClicked()
    {
        _buttonSound.Play();
    }

    private void OnSelectCharClicked()
    {
        _buttonSound.Play();
        var selectedItems = _charList.GetSelectedItems();
        if (selectedItems.Length == 0)
        {
            _selectLabel.Text = "请选择人物。";
            return;
        }
        var name = _charList.GetItemText(selectedItems[0]);
        _connection.WriteAndFlush(new LoginCharacterRequest(name));
        LoggedIn?.Invoke();
    }


    private void OnSexBoxChecked(CheckBox checkBox)
    {
        _femaleCheckBox.SetPressedNoSignal(false);
        _maleCheckBox.SetPressedNoSignal(false);
        checkBox.SetPressedNoSignal(true);
    }


    private void ConnectPressed()
    {
        _buttonSound.Play();
        if (string.IsNullOrEmpty(_username.Text))
        {
            _label.Text = "请输入用户名";
            return;
        }

        if (string.IsNullOrEmpty(_password.Text))
        {
            _label.Text = "请输入密码";
            return;
        }
        _label.Text = "登陆中...";
        _connection.WriteAndFlush(new LoginAccountRequest(_username.Text, _password.Text));
    }

    public void OnConnected(Connection connection)
    {
        if (DevMode)
        {
            connection.WriteAndFlush(new LoginCharacterRequest(""));
            LoggedIn?.Invoke();
            return;
        }
        _connection = connection;
        _label.Text = "连接成功";
        _regButton.Disabled = false;
        _connect.Disabled = false;
        _timer.Start(0.1f);
    }
}