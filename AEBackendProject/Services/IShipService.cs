using AEBackendProject.DTO.Port;
using AEBackendProject.Models;

namespace AEBackendProject.Services
{
    public interface IShipService
    {
        Task<Ship> GetByIdAsync(Guid id);
        Task<IEnumerable<Ship>> GetAllAsync();
        Task<IEnumerable<Ship>> GetAsync(string nameFilter, int pageNumber, int pageSize);
        Task CreateAsync(Ship ship);
        Task UpdateAsync(Ship ship);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Ship>> GetShipsAssignedToUser(Guid userId);
        Task<IEnumerable<Ship>> GetShipUnassigned();
        Task<ClosestPortResultDto> GetClosestPort(Guid shipId);
    }
}
