using Microsoft.EntityFrameworkCore;

namespace DAL.Data;
public class GenericRepository<TEntity> : IGenericRepository<TEntity>
    where TEntity : class
{
    protected readonly DbSet<TEntity> entities;

    public GenericRepository(AuctionDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        this.entities = context.Set<TEntity>();
    }

    public TEntity GetById(int id)
    {
        return entities.Find(id);
    }

    public void Add(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        entities.Add(entity);
    }

    public void Update(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        entities.Update(entity);
    }

    public void Remove(int id)
    {
        var entity = GetById(id);
        if (entity != null)
        {
            entities.Remove(entity);
        }
    }

    public List<TEntity> GetAll()
    {
        return entities.ToList();
    }

    public IQueryable<TEntity> GetQueryable()
    {
        return entities.AsQueryable();
    }
}
