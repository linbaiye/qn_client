using System;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Godot;
using NLog;
using QnClient.code.entity;
using QnClient.code.message;
using QnClient.code.player;
using Source.Networking.Protobuf;

namespace QnClient.code.network;

public class MessageDecoder() : LengthFieldBasedFrameDecoder(short.MaxValue, 0, 4, 0, 4)
{
    private static readonly ILogger Logger  = LogManager.GetCurrentClassLogger();
    
    private enum PositionType
    {
        Move = 1,
        Set = 3,
    }


    private object DecodePosition(PositionPacket packet)
    {
        return (PositionType)packet.Type switch
        {
            PositionType.Move => new MoveMessage(new Vector2I(packet.X, packet.Y), (CreatureDirection) packet.Direction, packet.Id, (MoveAction)packet.MoveAction),
            PositionType.Set => new SetPositionMessage(packet.Id, new Vector2I(packet.X, packet.Y), (CreatureDirection) packet.Direction, (PlayerState)packet.State),
            _ => null,
        };
    }
    
    private object Decode(Packet packet)
    {
        return packet.TypedPacketCase switch
        {
            Packet.TypedPacketOneofCase.JoinRealm => JoinRealmMessage.Parse(packet.JoinRealm),
            Packet.TypedPacketOneofCase.PositionPacket => DecodePosition(packet.PositionPacket),
            Packet.TypedPacketOneofCase.NpcSnapshot => NpcSnapshot.FromPacket(packet.NpcSnapshot),
            Packet.TypedPacketOneofCase.MonsterMove => MoveMessage.FromPacket(packet.MonsterMove),
            Packet.TypedPacketOneofCase.ChangeStatePacket => NpcChangeStateMessage.FromPacket(packet.ChangeStatePacket),
            Packet.TypedPacketOneofCase.RemoveEntity => new RemoveEntityMessage(packet.RemoveEntity.Id),
            Packet.TypedPacketOneofCase.KungFuBook => KungFuBookMessage.FromPacket(packet.KungFuBook),
            Packet.TypedPacketOneofCase.Text => new TextMessage(packet.Text.Text),
            Packet.TypedPacketOneofCase.Inventory => InventoryMessage.FromPacket(packet.Inventory),
            Packet.TypedPacketOneofCase.Equip => PlayerEquipMessage.FromPacket(packet.Equip),
            Packet.TypedPacketOneofCase.PlayerSnapshot => PlayerSnapshot.FromPacket(packet.PlayerSnapshot),
            Packet.TypedPacketOneofCase.Say => new CreatureSayMessage(packet.Say.Id, packet.Say.Text),
            Packet.TypedPacketOneofCase.ActiveKungFuList => SyncActiveKungFuListMessage.FromPacket(packet.ActiveKungFuList),
            Packet.TypedPacketOneofCase.PlayerChangeState => PlayerChangeStateMessage.FromPacket(packet.PlayerChangeState),
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