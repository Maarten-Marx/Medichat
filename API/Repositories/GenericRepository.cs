using System.Linq.Expressions;
using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class GenericRepository<T> : IRepository<T> where T : class
{
    private readonly MedichatContext context;
    private readonly DbSet<T> table;

    public GenericRepository(MedichatContext context)
    {
        this.context = context;
        table = this.context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await table.ToListAsync();
    }


    public async Task<T?> GetByIdAsync(int id)
    {
        return await table.FindAsync(id);
    }


    public async Task<IEnumerable<T>> GetAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params Expression<Func<T, object?>>[] includes
    )
    {
        IQueryable<T> query = table;

        foreach (var include in includes)
            query = query.Include(include);

        if (filter != null)
            query = query.Where(filter);

        if (orderBy != null)
            query = orderBy(query);

        return await query.ToListAsync();
    }

    public void Insert(T obj)
    {
        table.Add(obj);
    }

    public void Update(T obj)
    {
        table.Attach(obj);
        context.Entry(obj).State = EntityState.Modified;
    }

    public void Delete(int id)
    {
        var existing = table.Find(id);
        if (existing != null) table.Remove(existing);
    }

    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }
}