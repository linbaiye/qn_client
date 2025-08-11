using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using QnClient.code.message;
using QnClient.code.network.toserver;
using QnClient.code.sprite;

namespace QnClient.code.network;

public class Connection(string ip, int port)  : SimpleChannelInboundHandler<object>
{
    private IChannel? _channel;

    private readonly List<object> _messages = new();

    public async void Close()
    {
        if (_channel == null)
            return;
        await _channel.CloseAsync();
    }

    public void WriteAndFlush(I2ServerMessage message)
    {
        _channel?.WriteAndFlushAsync(message);
    }

    public override void ChannelInactive(IChannelHandlerContext context)
    {
        lock (_messages)
        {
            _messages.Add(DisconnectedMessage.Instance);
        }
    }


    public List<object> DrainMessages()
    {
        List<object> messages = new List<object>();
        lock (_messages)
        {
            messages.AddRange(_messages);
            _messages.Clear();
        }
        return messages;
    }

    private async Task Init()
    {
        Bootstrap bootstrap = new Bootstrap();
        bootstrap.Group(new SingleThreadEventLoop()).Handler(
            new ActionChannelInitializer<ISocketChannel>(c => c.Pipeline.AddLast(
                new LengthFieldPrepender(4), 
                new MessageEncoder(),
                new MessageDecoder(),
                this
            ))).Channel<TcpSocketChannel>();
        _channel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(ip), port));
    }

    public static async Task<Connection> ConnectTo(string ip, int port)
    {
        var connection = new Connection(ip, port);
        await connection.Init();
        return connection;
    }

    private async void LoadSprites(ISpriteMessage message)
    {
        foreach (var sprite in message.Sprites)
        {
            await Task.Run(() => ZipFileSpriteLoader.Instance.Load(sprite));
        }
        lock (_messages)
        {
            _messages.Add(message);
        }
    }

    protected override void ChannelRead0(IChannelHandlerContext ctx, object msg)
    {
        if (msg is ISpriteMessage spriteMessage)
        {
            LoadSprites(spriteMessage);
        }
        else
        {
            lock (_messages)
            {
                _messages.Add(msg);
            }
        }
    }
}