using System.Collections.Generic;
using QnClient.code.entity.@event;

namespace QnClient.code.entity;

public class EntityManager
{

    private readonly Dictionary<long, IEntity> _entities = new ();


    public void Add(IEntity entity)
    {
        _entities.TryAdd(entity.Id, entity);
    }

    public IEntity? Find(long id)
    {
        return _entities.GetValueOrDefault(id);
    }
    
    public AbstractCreature? Find(string name)
    {
        foreach (var entitiesValue in _entities.Values)
        {
            if (entitiesValue is AbstractCreature creature && creature.EntityName.Equals(name))
            {
                return creature;
            }
        }
        return null;
    }

    public void HandleEntityEvent(IEntityEvent entityEvent)
    {
        if (entityEvent is DeletedEvent)
        {
            _entities.Remove(entityEvent.Source.Id);
        }
    }

}