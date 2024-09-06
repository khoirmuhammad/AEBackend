using AEBackendProject.Data;
using AEBackendProject.Models;

namespace AEBackendProject.Repositories
{
    public class UserShipRepository : Repository<UserShip, int>, IUserShipRepository
    {
        public UserShipRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
