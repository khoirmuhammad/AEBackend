using AEBackendProject.Data;
using AEBackendProject.Models;

namespace AEBackendProject.Repositories
{
    public class ShipRepository : Repository<Ship, Guid>, IShipRepository
    {
        public ShipRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
