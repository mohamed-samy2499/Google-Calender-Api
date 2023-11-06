using System.Linq.Expressions;

namespace CalenderSystem.Infrastructure.Generic
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
        Task<string> CreateAsync(T entity);
        Task<string> DeleteAsync(int id);

        Task<T> UpdateAsync(T entity);
    }
}
