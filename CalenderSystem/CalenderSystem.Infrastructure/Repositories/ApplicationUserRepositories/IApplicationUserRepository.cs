
using CalenderSystem.Domain.Entities.Identity;
using System.Linq.Expressions;

namespace CalenderSystem.Infrastructure.Repositories.ApplicationUserRepositories
{
	public interface IApplicationUserRepository
	{
		Task<IEnumerable<ApplicationUser>> GetAllAsync(params Expression<Func<ApplicationUser, object>>[] includes);
		Task<ApplicationUser> GetByIdAsync(string id, params Expression<Func<ApplicationUser, object>>[] includes);
        Task<ApplicationUser> GetByEmailAsync(string email, params Expression<Func<ApplicationUser, object>>[] includes);

        Task<string> CreateAsync(ApplicationUser entity);
		Task<string> DeleteAsync(string id);

		Task<ApplicationUser> UpdateAsync(ApplicationUser entity);
	}
}
