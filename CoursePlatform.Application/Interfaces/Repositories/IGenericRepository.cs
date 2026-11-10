using System.Linq.Expressions;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Interfaces.Repositories;

public interface IGenericRepository<T>
    where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByIdAsync(
    int id,
    params Expression<Func<T, object>>[] includes);

    Task<IReadOnlyList<T>> GetAllAsync(
        Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includes);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<T?> FirstOrDefaultAsync(
    Expression<Func<T, bool>> predicate);

    Task<IReadOnlyList<T>> GetAllAsync(
        Expression<Func<T, bool>> predicate);
    Task<(
   IReadOnlyList<T > Items ,  int TotalCount)> GetPagedAsync(
       int pageNumber,
       int pageSize, Expression<Func<T, bool>>? filter = null, params Expression<Func<T, object>>[] includes);

    Task AddAsync(T entity);

    void Update(T entity);

    void Delete(T entity);
    public IQueryable<T> GetAllQuerable();

}