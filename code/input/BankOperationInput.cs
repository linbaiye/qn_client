using QnClient.code.network.toserver;
using Source.Networking.Protobuf;

namespace QnClient.code.input;

public class BankOperationInput(int type, long npcId, int fromSlot, int toSlot, int number) : I2ServerMessage
{
    public ClientPacket ToPacket()
    {
        return new ClientPacket()
        {
            BankOperation = new BankOperationInputPacket()
            {
                Type = type,
                NpcId = npcId,
                FromSlot = fromSlot,
                ToSlot = toSlot,
                Number = number
            }
        };
    }

    public static BankOperationInput InventoryToBank(long npcId, int fromSlot, int toSlot, int number)
    {
        return new BankOperationInput(1, npcId, fromSlot, toSlot, number);
    }
    
    public static BankOperationInput InventoryToBank(long npcId, int fromSlot, int number)
    {
        return new BankOperationInput(4, npcId, fromSlot, 0, number);
    }
    
    public static BankOperationInput BankToInventory(long npcId, int fromSlot, int toSlot, int number)
    {
        return new BankOperationInput(3, npcId, fromSlot, toSlot, number);
    }

    public static BankOperationInput BankToInventory(long npcId, int fromSlot, int number)
    {
        return new BankOperationInput(5, npcId, fromSlot, 0, number);
    }

    public static BankOperationInput Swap(long npcId, int fromSlot, int toSlot)
    {
        return new BankOperationInput(2, npcId, fromSlot, toSlot, 0);
    }

    public static BankOperationInput RightClick(long npcId, int slot)
    {
        return new BankOperationInput(6, npcId, slot, 0, 0);
    }
}