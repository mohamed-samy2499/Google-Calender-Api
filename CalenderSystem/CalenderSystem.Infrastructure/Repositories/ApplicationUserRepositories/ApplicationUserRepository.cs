using CalenderSystem.Domain.Entities.Identity;
using CalenderSystem.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CalenderSystem.Infrastructure.Repositories.ApplicationUserRepositories
{
	internal class ApplicationUserRepository: IApplicationUserRepository
	{
		private readonly AppDbContext _appDbContext;

		public ApplicationUserRepository(AppDbContext appDbContext)
		{
			_appDbContext = appDbContext;
		}


		public virtual async Task<string> CreateAsync(ApplicationUser entity)
		{
			await _appDbContext.Set<ApplicationUser>().AddAsync(entity);
			await _appDbContext.SaveChangesAsync();
			return "success";
		}

		public virtual async Task<ApplicationUser> UpdateAsync(ApplicationUser entity)
		{

			_appDbContext.Entry(entity).State = EntityState.Modified;
			await _appDbContext.SaveChangesAsync();
			return entity;
		}

		public virtual async Task<string> DeleteAsync(string id)
		{
			var entity = await _appDbContext.Set<ApplicationUser>().FindAsync(id);

			if (entity != null)
			{
				_appDbContext.Entry(entity).State = EntityState.Deleted;
				await _appDbContext.SaveChangesAsync();
				return "success";
			}
			return "Fails";
		}

		public async Task<IEnumerable<ApplicationUser>> GetAllAsync(Expression<Func<ApplicationUser, object>>[] includes = null)
		{
			IQueryable<ApplicationUser> query = _appDbContext.Set<ApplicationUser>();//.Where(x => x.IsDeleted == false);

			foreach (var include in includes)
			{
				query = query.Include(include);
			}

			return await query.ToListAsync();

		}

		public virtual async Task<ApplicationUser> GetByIdAsync(string id, Expression<Func<ApplicationUser, object>>[] includes = null)
		{
			IQueryable<ApplicationUser> query = _appDbContext.Set<ApplicationUser>();//.Where(x => x.IsDeleted == false);

			foreach (var include in includes)
			{
				query = query.Include(include);
			}

			var entity = await query.FirstOrDefaultAsync(x => x.Id == id);

			return entity;
		}
	}
}
