using System.Collections.Generic;
using QnClient.code.message;

namespace QnClient.code.hud.bottom;

public class MessageHistory(int max)
{
    private readonly List<TextMessage> _messages = new();

    public void Add(TextMessage msg)
    {
        if (_messages.Count > max)
            _messages.RemoveAt(_messages.Count - 1);
        _messages.Add(msg);
    }

    public List<TextMessage> GetAll => _messages;
    
    public List<TextMessage> Last5Messages()
    {
        List<TextMessage> ret = new List<TextMessage>();
        for (int e = _messages.Count - 1; e >= 0; e--)
        {
            ret.Add(_messages[e]);
        }
        ret.Reverse();
        return ret;
    }
}