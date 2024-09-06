using AEBackendProject.Common;
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
        public ShipService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
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
                throw new Exception($"No ship {shipId} found");

            var ports = await _unitOfWork.PortRepository.GetAllAsync();
            if (ports == null || !ports.Any())
                throw new Exception($"No ports found");

            // 1 knot = 1,852 km/hour
            double shipSpeedInHour = ship.Velocity * Constant.Knot;

            foreach (var port in ports)
            {
                // Get distance between ship and port location in kilometer
                double distanceInKm = GetDistanceInKilometer(ship.Latitude, ship.Longitude, port.Latitude, port.Longitude);

                // Get time = distance / speed
                double shipTime = distanceInKm / shipSpeedInHour;

                portDistanceTimeDtos.Add(new PortDistanceTimeDto
                {
                    PortName = port.Name,
                    Latitude = port.Latitude,
                    Longitude = port.Longitude,
                    Distance = distanceInKm,
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

            DistanceDto distanceDto = new DistanceDto
            {
                Kilometer = (int)closestPort.Distance,
                Meter = (closestPort.Distance - ((int)closestPort.Distance)) * 1000
            };

            return new ClosestPortResultDto
            {
                Ship = _mapper.Map<ShipDto>(ship),
                Port = _mapper.Map<PortDto>(closestPort),
                Distance = distanceDto,
                Time = timeDto
            };
        }

        //Haversine Formula
        private double GetDistanceInKilometer (double shipLat, double shipLon, double portLat, double portLon)
        {
            // Convert Latitude and Longitude to Radians: Latitude and longitude values need to be converted from degrees to radians before using them in the formula
            double latShipRad = ToRadians(shipLat);
            double lonShipRad = ToRadians(shipLon);
            double latPortRad = ToRadians(portLat);
            double lonPortRad = ToRadians(portLon);

            // Calculate the Differences: Compute the differences in latitude and longitude between the two points
            double latitudeDifference = latPortRad - latShipRad;
            double longitudeDifference = lonPortRad - lonShipRad;

            double centralAnglePart = (Math.Pow(Math.Sin(latitudeDifference / 2), 2)) +
                                      ((Math.Cos(latShipRad) * Math.Cos(latPortRad)) * Math.Pow(Math.Sin(longitudeDifference / 2), 2));

            double angularDistance = 2 * Math.Atan2(Math.Sqrt(centralAnglePart), Math.Sqrt(1 - centralAnglePart));

            // Return the distance in kilometers
            return Constant.EarthRadiusKm * angularDistance;
        }

        private double ToRadians(double degree)
        {
            return degree * (Math.PI / 180.0);
        }
    }
}
