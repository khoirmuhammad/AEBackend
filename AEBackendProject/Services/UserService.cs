using AEBackendProject.Models;
using AEBackendProject.Repositories;
using System.Linq.Expressions;

namespace AEBackendProject.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AssignShipToUser(Guid userId, Guid shipId)
        {
            IEnumerable<UserShip> userShip = await GetExistingUserShip(userId, shipId);

            if (userShip != null && !userShip.Any())
            {
                var newUserShip = new UserShip
                {
                    UserId = userId,
                    ShipId = shipId
                };

                await _unitOfWork.UserShipRepository.AddAsync(newUserShip);
                await _unitOfWork.CompleteAsync();
            }
            else
            {
                throw new Exception("Ship has assigned to user already");
            }
        }

        public async Task UnAssignShipToUser(Guid userId, Guid shipId)
        {
            IEnumerable<UserShip> userShip = await GetExistingUserShip(userId, shipId);

            if (userShip != null && userShip.Any())
            {
                await _unitOfWork.UserShipRepository.DeleteAsync(userShip.First());
                await _unitOfWork.CompleteAsync();
            }
        }

        private async Task<IEnumerable<UserShip>> GetExistingUserShip(Guid userId, Guid shipId)
        {
            var user = await GetByIdAsync(userId);
            var ship = await _unitOfWork.ShipRepository.GetByIdAsync(shipId);

            if (user == null)
                throw new Exception($"User Id {userId} not found");

            if (ship == null)
                throw new Exception($"Ship Id {shipId} not found");

            var userShip = await _unitOfWork.UserShipRepository.GetAsync(n => n.ShipId == shipId && n.UserId == userId);
            return userShip;
        }

        public async Task CreateAsync(User user)
        {
            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if(user != null)
            {
                user.IsDeleted = true;
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _unitOfWork.UserRepository.GetAsync(includes: new Expression<Func<User, object>>[] { u => u.UserShips });
        }

        public async Task<IEnumerable<User>> GetAsync(string nameFilter, int pageNumber, int pageSize)
        {
            return await _unitOfWork.UserRepository.GetAsync(
                predicate: u => u.Name.Contains(nameFilter),
                orderBy: q => q.OrderBy(u => u.Name),
                selector: s => s.Select(u => new User
                {
                    Id = u.Id,
                    Name = u.Name,
                }),
                skip: (pageNumber - 1) * pageSize,
                take: pageSize
            );
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.UserRepository.GetByIdAsync(id);
        }

        public async Task UpdateAsync(User user)
        {
            await _unitOfWork.UserRepository.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();
        }
    }
}
