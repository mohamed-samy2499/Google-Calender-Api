
using CalenderSystem.Domain.Entities.Identity;

namespace CalenderSystem.Application.IServices
{
    public interface IApplicationUserService
    {
        Task<IEnumerable<ApplicationUser>> GetAllApplicationUsersAsync();
        Task<ApplicationUser> GetApplicationUserByIdAsync(string id);

        Task<string> AddApplicationUserAsync(ApplicationUser Dto);
        Task<ApplicationUser> UpdateApplicationUserAsync(ApplicationUser Dto);
        Task<string> DeleteApplicationUserAsync(string id);
    }
}
