using AEBackendProject.DTO.Port;
using AEBackendProject.DTO.Ship;
using AEBackendProject.Models;
using AEBackendProject.Repositories;
using AEBackendProject.Services;
using AEBackendProject.Test.Helpers;
using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AEBackendProject.Test.Services
{
    public class ShipServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IConverter> _converterMock;
        private readonly Mock<IShipRepository> _shipRepoMock;
        private readonly Mock<IPortRepository> _portRepository;
        private readonly Mock<IUserShipRepository> _userShipRepoMock;
        private readonly ShipService _shipService;

        public ShipServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _converterMock = new Mock<IConverter>();
            _shipRepoMock = new Mock<IShipRepository>();
            _userShipRepoMock = new Mock<IUserShipRepository>();
            _portRepository = new Mock<IPortRepository>();

            _unitOfWorkMock.Setup(uow => uow.ShipRepository).Returns(_shipRepoMock.Object);
            _unitOfWorkMock.Setup(uow => uow.UserShipRepository).Returns(_userShipRepoMock.Object);
            _unitOfWorkMock.Setup(uow => uow.PortRepository).Returns(_portRepository.Object);
            _shipService = new ShipService(_mapperMock.Object, _unitOfWorkMock.Object, _converterMock.Object);
        }

        #region CreateAsync
        [Fact]
        public async Task CreateAsync_ShouldCallAddAndComplete()
        {
            // Arrange
            var ship = new Ship { Id = Guid.NewGuid(), Name = ConstantHelper.ShipTestName };

            // Act
            await _shipService.CreateAsync(ship);

            // Assert
            _shipRepoMock.Verify(repo => repo.AddAsync(ship), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        }
        #endregion

        #region DeleteAsync
        [Fact]
        public async Task DeleteAsync_MarkAsDeleted()
        {
            // Arrange
            var shipId = Guid.NewGuid();
            var ship = new Ship { Id = shipId };
            _unitOfWorkMock.Setup(uow => uow.ShipRepository.GetByIdAsync(shipId)).ReturnsAsync(ship);

            // Act
            await _shipService.DeleteAsync(shipId);

            // Assert
            Assert.True(ship.IsDeleted);
            _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        }
        #endregion

        #region GetAllAsync
        [Fact]
        public async Task GetAllAsync_ShouldReturnShips()
        {
            // Arrange
            var ships = new List<Ship> { new Ship(), new Ship() };
            _unitOfWorkMock.Setup(uow => uow.ShipRepository.GetAllAsync()).ReturnsAsync(ships);

            // Act
            var result = await _shipService.GetAllAsync();

            // Assert
            Assert.Equal(ships, result);
        }
        #endregion

        #region GetAsync
        [Fact]
        public async Task GetAsync_ShouldReturnFilteredShips()
        {
            // Arrange
            var ships = new List<Ship> { new Ship { Name = ConstantHelper.ShipTestName }, new Ship { Name = ConstantHelper.ShipTestName2 } };
            var nameFilter = ConstantHelper.ShipTestName;
            var pageNumber = 1;
            var pageSize = 10;

            _shipRepoMock.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<Ship, bool>>>(),
                It.IsAny<Func<IQueryable<Ship>, IOrderedQueryable<Ship>>>(),
                It.IsAny<Func<IQueryable<Ship>, IQueryable<Ship>>>(),
                It.IsAny<Expression<Func<Ship, object>>[]>(),
                It.IsAny<int?>(),
                It.IsAny<int?>()
            )).ReturnsAsync((Expression<Func<Ship, bool>> predicate,
                        Func<IQueryable<Ship>, IOrderedQueryable<Ship>> orderBy,
                        Func<IQueryable<Ship>, IQueryable<Ship>> selector,
                        Expression<Func<Ship, object>>[] includes,
                        int? skip,
                        int? take) =>
            {
                return ships.Where(s => s.Name.Contains(nameFilter)).ToList();
            });


            // Act
            var result = await _shipService.GetAsync(nameFilter, pageNumber, pageSize);

            // Assert
            Assert.Single(result);
            Assert.Equal(ConstantHelper.ShipTestName, result.First().Name);
        }
        #endregion

        #region GetByIdAsync
        [Fact]
        public async Task GetByIdAsync_ShouldReturnShip()
        {
            // Arrange
            var shipId = Guid.NewGuid();
            var ship = new Ship { Id = shipId };
            _unitOfWorkMock.Setup(uow => uow.ShipRepository.GetByIdAsync(shipId)).ReturnsAsync(ship);

            // Act
            var result = await _shipService.GetByIdAsync(shipId);

            // Assert
            Assert.Equal(ship, result);
        }
        #endregion

        #region GetShipsAssignedToUser
        [Fact]
        public async Task GetShipsAssignedToUser_ShouldReturnShips()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userShips = new List<UserShip> { new UserShip { ShipId = Guid.NewGuid() } };
            var ships = new List<Ship> { new Ship { Id = userShips.First().ShipId } };

            _userShipRepoMock.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<UserShip, bool>>>(),
                It.IsAny<Func<IQueryable<UserShip>, IOrderedQueryable<UserShip>>>(),
                It.IsAny<Func<IQueryable<UserShip>, IQueryable<UserShip>>>(),
                It.IsAny<Expression<Func<UserShip, object>>[]>(),
                It.IsAny<int?>(),
                It.IsAny<int?>()
            )).ReturnsAsync((Expression<Func<UserShip, bool>> predicate,
                        Func<IQueryable<UserShip>, IOrderedQueryable<UserShip>> orderBy,
                        Func<IQueryable<UserShip>, IQueryable<UserShip>> selector,
                        Expression<Func<UserShip, object>>[] includes,
                        int? skip,
                        int? take) =>
            {
                return userShips.Where(predicate.Compile()).ToList();
            });

            _shipRepoMock.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<Ship, bool>>>(),
                It.IsAny<Func<IQueryable<Ship>, IOrderedQueryable<Ship>>>(),
                It.IsAny<Func<IQueryable<Ship>, IQueryable<Ship>>>(),
                It.IsAny<Expression<Func<Ship, object>>[]>(),
                It.IsAny<int?>(),
                It.IsAny<int?>()
            )).ReturnsAsync((Expression<Func<Ship, bool>> predicate,
                        Func<IQueryable<Ship>, IOrderedQueryable<Ship>> orderBy,
                        Func<IQueryable<Ship>, IQueryable<Ship>> selector,
                        Expression<Func<Ship, object>>[] includes,
                        int? skip,
                        int? take) =>
            {
                return ships.ToList();
            });

            // Act
            var result = await _shipService.GetShipsAssignedToUser(userId);

            // Assert
            Assert.Single(result);
            Assert.Equal(ships.First().Id, result.First().Id);
        }
        #endregion

        #region GetShipUnassigned
        [Fact]
        public async Task GetShipUnassigned_ShouldReturnUnassignedShips()
        {
            // Arrange
            var allShips = new List<Ship> { new Ship { Id = Guid.NewGuid() }, new Ship { Id = Guid.NewGuid() } };
            var assignedShips = new List<UserShip> { new UserShip { ShipId = allShips.First().Id } };

            _unitOfWorkMock.Setup(uow => uow.ShipRepository.GetAllAsync()).ReturnsAsync(allShips);
            _userShipRepoMock.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<UserShip, bool>>>(),
                It.IsAny<Func<IQueryable<UserShip>, IOrderedQueryable<UserShip>>>(),
                It.IsAny<Func<IQueryable<UserShip>, IQueryable<UserShip>>>(),
                It.IsAny<Expression<Func<UserShip, object>>[]>(),
                It.IsAny<int?>(),
                It.IsAny<int?>()
            )).ReturnsAsync((Expression<Func<UserShip, bool>> predicate,
                        Func<IQueryable<UserShip>, IOrderedQueryable<UserShip>> orderBy,
                        Func<IQueryable<UserShip>, IQueryable<UserShip>> selector,
                        Expression<Func<UserShip, object>>[] includes,
                        int? skip,
                        int? take) =>
            {
                return assignedShips.ToList();
            });

            // Act
            var result = await _shipService.GetShipUnassigned();

            // Assert
            Assert.Single(result);
            Assert.NotEqual(allShips.First().Id, result.First().Id);
        }
        #endregion

        #region UpdateAsync
        [Fact]
        public async Task UpdateAsync_ShouldCallUpdateAndComplete()
        {
            // Arrange
            var ship = new Ship();
            _unitOfWorkMock.Setup(uow => uow.ShipRepository.UpdateAsync(ship)).Returns(Task.CompletedTask);

            // Act
            await _shipService.UpdateAsync(ship);

            // Assert
            _unitOfWorkMock.Verify(uow => uow.ShipRepository.UpdateAsync(ship), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        }
        #endregion

        #region GetClosestPort
        [Fact]
        public async Task GetClosestPort_ShouldReturnClosestPort()
        {
            // Arrange
            var shipId = Guid.NewGuid();
            var ship = new Ship { Id = shipId, Name = ConstantHelper.ShipTestName, Latitude = 10, Longitude = 10, Velocity = 20 };
            var ports = new List<Port>
            {
                new Port { Name = ConstantHelper.PortA, Latitude = 40, Longitude = 20 },
                new Port { Name = ConstantHelper.PortB, Latitude = 30, Longitude = 60 }
            };

            var shipDto = new ShipDto { Id = shipId, Name = ConstantHelper.ShipTestName };
            var portDto = new PortDto { Name = ConstantHelper.PortA };
            var distanceDto = new DistanceDto { Distance = 135.68, Measure = ConstantHelper.KM };

            _shipRepoMock.Setup(repo => repo.GetByIdAsync(shipId)).ReturnsAsync(ship);
            _portRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(ports);
            _converterMock.Setup(c => c.GetSpeed(ship.Velocity)).Returns(1.852);
            _converterMock.Setup(c => c.GetDistance(It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>())).Returns(135.68);
            _converterMock.Setup(c => c.GetDistanceDto(It.IsAny<double>())).Returns((double distance) => distanceDto);
            _mapperMock.Setup(m => m.Map<ShipDto>(It.IsAny<Ship>())).Returns(shipDto);
            _mapperMock.Setup(m => m.Map<PortDto>(It.IsAny<PortDistanceTimeDto>())).Returns(portDto);
            // Act
            var result = await _shipService.GetClosestPort(shipId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ConstantHelper.PortA, result.ClosestPort.Name);
        }
        #endregion
    }
}
