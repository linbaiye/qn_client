using System;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NLog;
using QnClient.code.message;
using Source.Networking.Protobuf;

namespace QnClient.code.network;

public class MessageDecoder() : LengthFieldBasedFrameDecoder(short.MaxValue, 0, 4, 0, 4)
{
    private static readonly ILogger Logger  = LogManager.GetCurrentClassLogger();
    
    private object Decode(Packet packet)
    {
        return packet.TypedPacketCase switch
        {
            Packet.TypedPacketOneofCase.LoginPacket => JoinRealmMessage.Parse(packet.LoginPacket),
            _ => null,
        };
    }
    
    protected override object Decode(IChannelHandlerContext context, IByteBuffer input)
    {
        try
        {
            IByteBuffer frame = (IByteBuffer)base.Decode(context, input);
            if (frame == null)
            {
                return null;
            }
            byte[] bytes = new byte[frame.ReadableBytes];
            frame.ReadBytes(bytes);
            Packet packet = Packet.Parser.ParseFrom(bytes);
            frame.Release();
            return Decode(packet);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to decode message.");
            throw new NotImplementedException();
        }
    }
}