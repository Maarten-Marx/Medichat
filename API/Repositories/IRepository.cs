using System.Linq.Expressions;

namespace API.Repositories;

public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    void Insert(T obj);
    void Delete(int id);
    void Update(T obj);
    Task SaveAsync();

    Task<IEnumerable<T>> GetAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params Expression<Func<T, object?>>[] includes
    );
}