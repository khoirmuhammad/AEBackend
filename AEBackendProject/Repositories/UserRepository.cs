using AEBackendProject.Data;
using AEBackendProject.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace AEBackendProject.Repositories
{
    public class UserRepository : Repository<User, Guid>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
