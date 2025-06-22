using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using NLog;
using QnClient.code.network.toserver;

namespace QnClient.code.network;


public class MessageEncoder : MessageToByteEncoder<I2ServerMessage>
{
    private static readonly ILogger Logger  = LogManager.GetCurrentClassLogger();
    
    protected override void Encode(IChannelHandlerContext context, I2ServerMessage message, IByteBuffer output)
    {
        output.WriteBytes(message.ToPacket().ToByteArray());
        Logger.Debug("Sent message.");
    }
}