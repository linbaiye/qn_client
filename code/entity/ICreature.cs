namespace QnClient.code.entity;

public interface ICreature : IEntity
{
    CreatureDirection Direction { get; set; }
    
}