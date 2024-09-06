using AEBackendProject.Models;

namespace AEBackendProject.Services
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(Guid id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetAsync(string nameFilter, int pageNumber, int pageSize);
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid id);
        Task AssignShipToUser(Guid userId, Guid shipId);
        Task UnAssignShipToUser(Guid userId, Guid shipId);
    }
}
