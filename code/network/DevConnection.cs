using System;
using System.Collections.Generic;
using QnClient.code.network.toserver;

namespace QnClient.code.network;

public class DevConnection : IConnection
{
    private List<Object> _messages = new List<object>();
    public List<object> DrainMessages()
    {
        List<object> messages = new List<object>();
        messages.AddRange(_messages);
        _messages.Clear();
        return messages;
    }

    public void WriteAndFlush(I2ServerMessage message)
    {
    }

    public void Add(object message)
    {
        _messages.Add(message);
    }
}