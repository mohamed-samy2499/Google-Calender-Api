using CalenderSystem.Application.IServices;
using CalenderSystem.Domain.Entities.Identity;
using CalenderSystem.Infrastructure.Repositories.ApplicationUserRepositories;

namespace MovieSystem.Application.Services
{
    public class ApplicationUserService : IApplicationUserService
	{
        #region CTOR
        private readonly IApplicationUserRepository _applicationUserRepository;
        public ApplicationUserService(IApplicationUserRepository applicationUserRepository)
        {
			_applicationUserRepository = applicationUserRepository;
        }
        #endregion

        public async Task<string> AddApplicationUserAsync(ApplicationUser Dto)
        {
            return await _applicationUserRepository.CreateAsync(Dto);
        }

        public async Task<string> DeleteApplicationUserAsync(string id)
        {
            return await _applicationUserRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllApplicationUsersAsync()
        {
            return await _applicationUserRepository.GetAllAsync();
        }

        public async Task<ApplicationUser> GetApplicationUserByIdAsync(string id)
        {
            return await _applicationUserRepository.GetByIdAsync(id);
        }

        public async Task<ApplicationUser> UpdateApplicationUserAsync(ApplicationUser Dto)
        {
            return await _applicationUserRepository.UpdateAsync(Dto);
        }
    }
}
