using System.Linq.Expressions;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Interfaces.Repositories;

public interface IGenericRepository<T>
    where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);

    Task<IReadOnlyList<T>> GetAllAsync();
    Task<(
   IReadOnlyList<T > Items ,  int TotalCount)> GetPagedAsync(
       int pageNumber,
       int pageSize, params Expression<Func<T, object>>[] includes);

    Task AddAsync(T entity);

    void Update(T entity);

    void Delete(T entity);
    public IQueryable<T> GetAllQuerable();

}