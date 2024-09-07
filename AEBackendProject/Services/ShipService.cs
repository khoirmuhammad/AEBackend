using AEBackendProject.Common;
using AEBackendProject.Common.Exceptions;
using AEBackendProject.DTO.Port;
using AEBackendProject.DTO.Ship;
using AEBackendProject.Models;
using AEBackendProject.Repositories;
using AutoMapper;

namespace AEBackendProject.Services
{
    public class ShipService : IShipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConverter _converter;
        public ShipService(IMapper mapper, IUnitOfWork unitOfWork, IConverter converter)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _converter = converter;
        }

        public async Task CreateAsync(Ship ship)
        {
            await _unitOfWork.ShipRepository.AddAsync(ship);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                user.IsDeleted = true;
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<IEnumerable<Ship>> GetAllAsync()
        {
            return await _unitOfWork.ShipRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Ship>> GetAsync(string nameFilter, int pageNumber, int pageSize)
        {
            return await _unitOfWork.ShipRepository.GetAsync(
                predicate: u => u.Name.Contains(nameFilter),
                orderBy: q => q.OrderBy(u => u.Name),
                selector: s => s.Select(u => new Ship
                {
                    Id = u.Id,
                    Name = u.Name,
                    Latitude = u.Latitude,
                    Longitude = u.Longitude,
                }),
                skip: (pageNumber - 1) * pageSize,
                take: pageSize
            );
        }

        public async Task<Ship> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.ShipRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Ship>> GetShipsAssignedToUser(Guid userId)
        {
            var userShips = await _unitOfWork.UserShipRepository.GetAsync(
                predicate: n => n.UserId == userId);

            var shipIds = userShips.Select(n => n.ShipId);

            var ships = await _unitOfWork.ShipRepository.GetAsync(
                predicate: n => shipIds.Contains(n.Id));

            return ships.ToList();
        }

        public async Task<IEnumerable<Ship>> GetShipUnassigned()
        {
            IEnumerable<Ship> ships = await _unitOfWork.ShipRepository.GetAllAsync();

            var userShips = await _unitOfWork.UserShipRepository.GetAsync(
                selector: s => s.Select(us => new UserShip
                {
                    ShipId = us.ShipId
                }).Distinct());

            var result = ships.Where(n => !userShips.Select(x => x.ShipId).Contains(n.Id));

            return result;
        }

        public async Task UpdateAsync(Ship ship)
        {
            await _unitOfWork.ShipRepository.UpdateAsync(ship);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<ClosestPortResultDto> GetClosestPort(Guid shipId)
        {
            List<PortDistanceTimeDto> portDistanceTimeDtos = new List<PortDistanceTimeDto>();

            var ship = await _unitOfWork.ShipRepository.GetByIdAsync(shipId);
            if (ship == null)
                throw new ItemNotFoundException($"No ship {shipId} found");

            var ports = await _unitOfWork.PortRepository.GetAllAsync();
            if (ports == null || !ports.Any())
                throw new ItemNotFoundException($"No ports found");

            // 1 knot = 1,852 km/hour
            double shipSpeedInHour = _converter.GetSpeed(ship.Velocity);

            foreach (var port in ports)
            {
                // Get distance between ship and port location in kilometer
                double distance = _converter.GetDistance(ship.Latitude, ship.Longitude, port.Latitude, port.Longitude);

                // Get time = distance (in KM) / speed
                double shipTime = distance / shipSpeedInHour;

                portDistanceTimeDtos.Add(new PortDistanceTimeDto
                {
                    PortName = port.Name,
                    Latitude = port.Latitude,
                    Longitude = port.Longitude,
                    Distance = distance,
                    Time = shipTime,
                });
            }

            var closestPort = portDistanceTimeDtos.OrderBy(o => o.Time).FirstOrDefault();

            TimeSpan time = TimeSpan.FromHours(closestPort.Time);
            TimeDto timeDto = new TimeDto
            {
                Hour = (int)time.TotalHours,
                Minute = time.Minutes,
                Second= time.Seconds,
            };

            var result = new ClosestPortResultDto
            {
                Ship = _mapper.Map<ShipDto>(ship),
                ClosestPort = _mapper.Map<PortDto>(closestPort),
                ShipPortDistance = _converter.GetDistanceDto(closestPort.Distance),
                EstimationTime = timeDto
            };
            return result;
        }
    }
}
