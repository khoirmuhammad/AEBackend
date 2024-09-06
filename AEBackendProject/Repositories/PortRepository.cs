using AEBackendProject.Data;
using AEBackendProject.Models;

namespace AEBackendProject.Repositories
{
    public class PortRepository : Repository<Port, Guid>, IPortRepository
    {
        public PortRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
