using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AEBackendProject.Common.Exceptions;
using AEBackendProject.Models;
using AEBackendProject.Repositories;
using AEBackendProject.Services;
using AEBackendProject.Test.Helpers;
using Moq;
using Xunit;

namespace AEBackendProject.Test.Services
{
    public class UserServiceTest
    {
        private readonly UserService _userService;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IShipRepository> _shipRepoMock;
        private readonly Mock<IUserShipRepository> _userShipRepoMock;

        public UserServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _userRepoMock = new Mock<IUserRepository>();
            _shipRepoMock = new Mock<IShipRepository>();
            _userShipRepoMock = new Mock<IUserShipRepository>();

            _unitOfWorkMock.Setup(uow => uow.UserRepository).Returns(_userRepoMock.Object);
            _unitOfWorkMock.Setup(uow => uow.ShipRepository).Returns(_shipRepoMock.Object);
            _unitOfWorkMock.Setup(uow => uow.UserShipRepository).Returns(_userShipRepoMock.Object);

            _userService = new UserService(_unitOfWorkMock.Object);
        }

        #region AssignShipToUser
        [Fact]
        public async Task AssignShipToUser_ThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var shipId = Guid.NewGuid();
            var existingUserShip = new List<UserShip>
            {
                new UserShip { UserId = userId, ShipId = shipId }
            };

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
                return existingUserShip.Where(predicate.Compile()).ToList();
            });

            _userRepoMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId });

            _shipRepoMock.Setup(repo => repo.GetByIdAsync(shipId)).ReturnsAsync(new Ship { Id = shipId });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ShipAlreadyAssignedException>(() => _userService.AssignShipToUser(userId, shipId));
            Assert.Equal($"Ship with Id {shipId} is already assigned to user with Id {userId}.", exception.Message);
        }

        [Fact]
        public async Task AssignShipToUser_Successfull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var shipId = Guid.NewGuid();
            var existingUserShip = new List<UserShip>();

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
                return existingUserShip.Where(predicate.Compile()).ToList();
            });

            _userRepoMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId });
            _shipRepoMock.Setup(repo => repo.GetByIdAsync(shipId)).ReturnsAsync(new Ship { Id = shipId });

            // Act
            await _userService.AssignShipToUser(userId, shipId);

            // Assert
            _userShipRepoMock.Verify(repo => repo.AddAsync(It.Is<UserShip>(us => us.UserId == userId && us.ShipId == shipId)), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        }
        #endregion

        #region UnAssignShipToUser
        [Fact]
        public async Task UnAssignShipToUser_Successful()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var shipId = Guid.NewGuid();
            var existingUserShip = new List<UserShip>
            {
                new UserShip { UserId = userId, ShipId = shipId }
            };

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
                return existingUserShip.Where(predicate.Compile()).ToList();
            });

            _userRepoMock.Setup(repo => repo.GetByIdAsync(userId))
                 .ReturnsAsync(new User { Id = userId });

            // Mock the GetByIdAsync for Ship
            _shipRepoMock.Setup(repo => repo.GetByIdAsync(shipId))
                         .ReturnsAsync(new Ship { Id = shipId });

            // Act
            await _userService.UnAssignShipToUser(userId, shipId);

            // Assert
            _userShipRepoMock.Verify(repo => repo.DeleteAsync(It.Is<UserShip>(us => us.UserId == userId && us.ShipId == shipId)), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        }
        #endregion

        #region CreateAsync
        [Fact]
        public async Task CreateAsync_AddUser()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Name = ConstantHelper.UserTestName };

            // Act
            await _userService.CreateAsync(user);

            // Assert
            _userRepoMock.Verify(repo => repo.AddAsync(user), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        }
        #endregion

        #region DeleteAsync
        [Fact]
        public async Task DeleteAsync_MarkUserAsDeleted()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, IsDeleted = false };

            _userRepoMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

            // Act
            await _userService.DeleteAsync(userId);

            // Assert
            Assert.True(user.IsDeleted);
            _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        }
        #endregion

        #region GetAllAsync
        [Fact]
        public async Task GetAllAsync_ReturnUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Name = ConstantHelper.UserTestName }
            };

            // Mock the GetAsync method
            _userRepoMock.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                It.IsAny<Func<IQueryable<User>, IQueryable<User>>>(),
                It.IsAny<Expression<Func<User, object>>[]>(), // Handle includes
                It.IsAny<int?>(),
                It.IsAny<int?>()
            )).ReturnsAsync(users);

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(ConstantHelper.UserTestName, result.First().Name);
        }
        #endregion

        #region GetAsync
        [Fact]
        public async Task GetAsync_ReturnFilteredUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Name = ConstantHelper.UserTestName }
            };

            _userRepoMock.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Func<IQueryable<User>, IOrderedQueryable<User>>>(),
                It.IsAny<Func<IQueryable<User>, IQueryable<User>>>(),
                It.IsAny<Expression<Func<User, object>>[]>(),
                It.IsAny<int?>(),
                It.IsAny<int?>()
            )).ReturnsAsync((Expression<Func<User, bool>> predicate,
                        Func<IQueryable<User>, IOrderedQueryable<User>> orderBy,
                        Func<IQueryable<User>, IQueryable<User>> selector,
                        Expression<Func<User, object>>[] includes,
                        int? skip,
                        int? take) =>
            {
                return users.Where(predicate.Compile()).ToList();
            });

            // Act
            var result = await _userService.GetAsync(ConstantHelper.UserTestName, 1, 10);

            // Assert
            Assert.Single(result);
            Assert.Equal(ConstantHelper.UserTestName, result.First().Name);
        }
        #endregion

        #region GetByIdAsync
        [Fact]
        public async Task GetByIdAsync_ReturnUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Name = ConstantHelper.UserTestName };

            _userRepoMock.Setup(repo => repo.GetByIdAsync(userId))
                         .ReturnsAsync(user);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ConstantHelper.UserTestName, result.Name);
        }
        #endregion

        #region UpdateAsync
        [Fact]
        public async Task UpdateAsync_UpdateUser()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Name = ConstantHelper.UserTestName };

            // Act
            await _userService.UpdateAsync(user);

            // Assert
            _userRepoMock.Verify(repo => repo.UpdateAsync(user), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CompleteAsync(), Times.Once);
        }
        #endregion
    }
}
