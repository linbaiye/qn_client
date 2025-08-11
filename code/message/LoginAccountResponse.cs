using System.Collections.Generic;
using Source.Networking.Protobuf;

namespace QnClient.code.message;

public class LoginAccountResponse(int code, string msg, List<string> charnames)
{
    public int Code { get; } = code;
    public string Msg { get; } = msg;
    public List<string> Charnames { get; } = charnames;

    public static LoginAccountResponse FromPacket(LoginResponsePacket packet)
    {
        List<string> names = new List<string>();
        foreach (var name in packet.Characters)
        {
            names.Add(name);
        }
        return new LoginAccountResponse(packet.Code, packet.Description, names);
    }
    
}