using System;
using Godot;
using QnClient.code.input;
using QnClient.code.message;
using QnClient.code.network;

namespace QnClient.code.hud;

public partial class Login : NinePatchRect
{
    private LineEdit _username;
    
    private LineEdit _password;
    
    private Button _connect;
    
    private Button _exit;

    private Label _label;
    private Button _regButton;

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
    
    
    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
        _username = GetNode<LineEdit>("Username");
        _password = GetNode<LineEdit>("Password");
        _connect = GetNode<Button>("Connect");
        _exit = GetNode<Button>("Exit");
        _exit.Pressed += () => Exited?.Invoke();
        _connect.Pressed += ConnectPressed;
        _regButton = GetNode<Button>("RegButton");
        _regButton.Pressed += () => _register.Visible = true;
        _register = GetNode<NinePatchRect>("Register");
        _register.Visible = false;
        _regLabel = _register.GetNode<Label>("Label");
        _regUsername = _register.GetNode<LineEdit>("Username");
        _regPassword = _register.GetNode<LineEdit>("Password");
        _regConfirmPassword = _register.GetNode<LineEdit>("ConfirmPassword");
        _regConfirm = _register.GetNode<Button>("Confirm");
        _regConfirm.Pressed += ConfirmRegister;
        _regReturn = _register.GetNode<Button>("Return");
        _regReturn.Pressed += () => _register.Visible = false;
        _regButton.Disabled = true;
        _connect.Disabled = true;
    }


    private void ConfirmRegister()
    {
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

    public override void _Process(double delta)
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
                if (loginAccountResponse.Code != 0)
                {
                    _label.Text = loginAccountResponse.Msg;
                }
                
            }
        }
    }


    private void ConnectPressed()
    {
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
        _connection.WriteAndFlush(new LoginAccountRequest(_username.Text, _password.Text));
        //LoggedIn?.Invoke();
        //_connection.WriteAndFlush(new DebugInput());
    }

    public void OnConnected(Connection connection)
    {
        _connection = connection;
        _label.Text = "连接成功。";
        _regButton.Disabled = false;
        _connect.Disabled = false;
    }
}