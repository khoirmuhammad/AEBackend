using AEBackendProject.Data;
using System;

namespace AEBackendProject.Repositories
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IShipRepository _shipRepository;
        private readonly IPortRepository _portRepository;
        private readonly IUserShipRepository _userShipRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            _userRepository ??= new UserRepository(_context);
            _shipRepository ??= new ShipRepository(_context);
            _portRepository ??= new PortRepository(_context);
            _userShipRepository ??= new UserShipRepository(_context);
        }

        public IUserRepository UserRepository => _userRepository;

        public IShipRepository ShipRepository => _shipRepository;

        public IPortRepository PortRepository => _portRepository;

        public IUserShipRepository UserShipRepository => _userShipRepository;

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
