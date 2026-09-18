using System.Transactions;
using CoursePlatform.Application.Interfaces.Repositories;
using CoursePlatform.Application.Interfaces.UnitOfWork;
using CoursePlatform.Domain.Entities;
using CoursePlatform.Infrastructure.Persistence.Repositories;

namespace CoursePlatform.Infrastructure.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<T> Repository<T>()
            where T : BaseEntity
        {
            return new GenericRepository<T>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task<bool> ExecuteInTransactionAllContextAsync(
Func<Task<bool>> action)
        {
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = await action();

                    if (result)
                        scope.Complete();

                    return result;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
