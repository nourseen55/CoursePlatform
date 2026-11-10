using System.Linq.Expressions;
using CoursePlatform.Application.Interfaces.Repositories;
using CoursePlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoursePlatform.Infrastructure.Persistence.Repositories;


public class GenericRepository<T> : IGenericRepository<T>
    where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }
    public async Task<T?> GetByIdAsync(
    int id,
    params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync(
            x => x.Id == id);
    }
    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<IReadOnlyList<T>> GetAllAsync(
    Expression<Func<T, bool>> predicate,
    params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet
            .AsNoTracking()
            .Where(predicate);

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }
    public  IQueryable<T> GetAllQuerable()
    {
        return  _dbSet
            .AsNoTracking()
            .AsQueryable();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
    int pageNumber,
    int pageSize, Expression<Func<T, bool>>? filter = null,
    params Expression<Func<T, object>>[] includes)
    {
        var query = _dbSet
          .AsNoTracking()
          .AsQueryable();
        if (filter != null)
        {
            query = query.Where(filter);
        }

        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        var totalCount = await query.CountAsync();

        var items = await query
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
    public async Task<IReadOnlyList<T>> GetAllAsync(
    Expression<Func<T, bool>> predicate)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate);
    }
}