using System;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using NLog;
using QnClient.code.message;
using QnClient.code.player;
using Source.Networking.Protobuf;

namespace QnClient.code.network;

public class MessageDecoder() : LengthFieldBasedFrameDecoder(short.MaxValue, 0, 4, 0, 4)
{
    private static readonly ILogger Logger  = LogManager.GetCurrentClassLogger();
    
    private object Decode(Packet packet)
    {
        return packet.TypedPacketCase switch
        {
            Packet.TypedPacketOneofCase.JoinRealm => JoinRealmMessage.Parse(packet.JoinRealm),
            Packet.TypedPacketOneofCase.NpcSnapshot => NpcSnapshot.FromPacket(packet.NpcSnapshot),
            Packet.TypedPacketOneofCase.NpcMove => MoveMessage.FromPacket(packet.NpcMove),
            Packet.TypedPacketOneofCase.PlayerMove => MoveMessage.FromPacket(packet.PlayerMove),
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
            Packet.TypedPacketOneofCase.Unequip => new PlayerUnequipMessage(packet.Unequip.Id, (EquipmentType)packet.Unequip.EquipmentType),
            Packet.TypedPacketOneofCase.Attribute => AttributeMessage.FromPacket(packet.Attribute),
            Packet.TypedPacketOneofCase.Attack => PlayerAttackMessage.FromPacket(packet.Attack),
            Packet.TypedPacketOneofCase.PositionPacket => SetPositionMessage.FromPacket(packet.PositionPacket),
            Packet.TypedPacketOneofCase.PlayerSetPosition => SetPositionMessage.FromPacket(packet.PlayerSetPosition),
            Packet.TypedPacketOneofCase.EntitySound => new SoundMessage(packet.EntitySound.EntityName, packet.EntitySound.Sound),
            Packet.TypedPacketOneofCase.PlayerDamaged => PlayerDamagedMessage.FromPacket(packet.PlayerDamaged),
            Packet.TypedPacketOneofCase.EntityDamaged => new EntityLifeBarMessage(packet.EntityDamaged.Id, packet.EntityDamaged.Percent),
            Packet.TypedPacketOneofCase.GainExp => packet.GainExp.KungFu ? new KungFuGainExpMessage(packet.GainExp.Level, packet.GainExp.Name) : new GainExpMessage(packet.GainExp.Name),
            Packet.TypedPacketOneofCase.UpdateSlot => InventoryItemMessage.FromPacket(packet.UpdateSlot),
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