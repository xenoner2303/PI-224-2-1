namespace DAL.Data;

public interface IGenericRepository<T>
    where T : class
{
    public T GetById(int id);
    public void Add(T entity);
    public void Update(T entity);
    public void Remove(int id);
    public List<T> GetAll();
    public IQueryable<T> GetQueryable();
}
