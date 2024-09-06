using AEBackendProject.Models;
using static AEBackendProject.Repositories.IRepository;

namespace AEBackendProject.Repositories
{
    public interface IShipRepository : IRepository<Ship, Guid>
    {
    }
}
