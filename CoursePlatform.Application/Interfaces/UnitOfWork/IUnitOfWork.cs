using CoursePlatform.Application.Interfaces.Repositories;
using CoursePlatform.Domain.Entities;

namespace CoursePlatform.Application.Interfaces.UnitOfWork
{
    public interface IUnitOfWork
    {
        IGenericRepository<T> Repository<T>()
            where T : BaseEntity;

        Task<int> SaveChangesAsync();
        Task<bool> ExecuteInTransactionAllContextAsync(Func<Task<bool>> action);

    }
}
