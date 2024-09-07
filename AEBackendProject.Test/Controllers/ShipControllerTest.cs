using AEBackendProject.Common;
using AEBackendProject.Controllers;
using AEBackendProject.DTO.Port;
using AEBackendProject.DTO.Ship;
using AEBackendProject.Models;
using AEBackendProject.Services;
using AEBackendProject.Test.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEBackendProject.Test.Controllers
{
    public class ShipControllerTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IShipService> _shipServiceMock;
        private readonly Mock<IResponseHelper> _responseHelperMock;
        private readonly ShipController _controller;

        public ShipControllerTest()
        {
            _mapperMock = new Mock<IMapper>();
            _shipServiceMock = new Mock<IShipService>();
            _responseHelperMock = new Mock<IResponseHelper>();
            _controller = new ShipController(_mapperMock.Object, _shipServiceMock.Object, _responseHelperMock.Object);
        }

        #region GetShipById
        [Fact]
        public async Task GetShipById_ReturnsNotFound_WhenShipIsNull()
        {
            // Arrange
            var shipId = Guid.NewGuid();
            _shipServiceMock.Setup(service => service.GetByIdAsync(shipId)).ReturnsAsync((Ship)null);
            _responseHelperMock.Setup(helper => helper.CreateNotFoundResponse(It.IsAny<string>()))
                .Returns(new NotFoundObjectResult(ConstantHelper.ItemNotFound));

            // Act
            var result = await _controller.GetShipById(shipId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(ConstantHelper.ItemNotFound, notFoundResult.Value);
        }

        [Fact]
        public async Task GetShipById_ReturnOk_WhenShipExists()
        {
            // Arrange
            var shipId = Guid.NewGuid();
            var ship = new Ship { Id = shipId, Name = ConstantHelper.ShipTestName };
            var shipDto = new ShipDto { Id = shipId, Name = ConstantHelper.ShipTestName };

            _shipServiceMock.Setup(service => service.GetByIdAsync(shipId)).ReturnsAsync(ship);
            _mapperMock.Setup(mapper => mapper.Map<ShipDto>(ship)).Returns(shipDto);
            _responseHelperMock.Setup(helper => helper.CreateOkResponse(shipDto))
                .Returns(new OkObjectResult(shipDto));

            // Act
            var result = await _controller.GetShipById(shipId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(shipDto, okResult.Value);
        }
        #endregion

        #region GetAllShips
        [Fact]
        public async Task GetAllShips_ReturnOk_WithShips()
        {
            // Arrange
            var ships = new List<Ship> { new Ship { Id = Guid.NewGuid(), Name = ConstantHelper.ShipTestName } };
            var shipDtos = new List<ShipDto> { new ShipDto { Id = ships[0].Id, Name = ConstantHelper.ShipTestName } };

            _shipServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(ships);
            _mapperMock.Setup(mapper => mapper.Map<List<ShipDto>>(ships)).Returns(shipDtos);
            _responseHelperMock.Setup(helper => helper.CreateOkResponse(shipDtos))
                .Returns(new OkObjectResult(shipDtos));

            // Act
            var result = await _controller.GetAllShips();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(shipDtos, okResult.Value);
        }
        #endregion

        #region CreateShip
        [Fact]
        public async Task CreateShip_ReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var request = new ShipCreateDto();
            _controller.ModelState.AddModelError("errorKey", ConstantHelper.ShipCreateBadRequest);
            _responseHelperMock.Setup(helper => helper.CreateBadRequestResponse(It.IsAny<IEnumerable<string>>()))
                .Returns(new BadRequestObjectResult(ConstantHelper.ShipCreateBadRequest));

            // Act
            var result = await _controller.CreateShip(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(ConstantHelper.ShipCreateBadRequest, badRequestResult.Value);
        }

        [Fact]
        public async Task CreateShip_ReturnCreatedResponse_WhenModelStateIsValid()
        {
            // Arrange
            var request = new ShipCreateDto();
            var ship = new Ship { Id = Guid.NewGuid() };
            _mapperMock.Setup(mapper => mapper.Map<Ship>(request)).Returns(ship);
            _shipServiceMock.Setup(service => service.CreateAsync(ship)).Returns(Task.CompletedTask);
            _responseHelperMock.Setup(helper => helper.CreateCreatedResponse(ship, ship.Id, nameof(ShipController), nameof(_controller.GetShipById)))
                .Returns(new CreatedAtActionResult(nameof(_controller.GetShipById), nameof(ShipController), new { id = ship.Id }, ship));

            // Act
            var result = await _controller.CreateShip(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(ship, createdResult.Value);
        }
        #endregion

        #region UpdateShipVelocity
        [Fact]
        public async Task UpdateShipVelocity_ReturnNotFound_WhenShipIsNull()
        {
            // Arrange
            var shipId = Guid.NewGuid();
            var newVelocity = 10.0;
            _shipServiceMock.Setup(service => service.GetByIdAsync(shipId)).ReturnsAsync((Ship)null);
            _responseHelperMock.Setup(helper => helper.CreateNotFoundResponse(It.IsAny<string>()))
                .Returns(new NotFoundObjectResult(ConstantHelper.ItemNotFound));

            // Act
            var result = await _controller.UpdateShipVelocity(shipId, newVelocity);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(ConstantHelper.ItemNotFound, notFoundResult.Value);
        }

        [Fact]
        public async Task UpdateShipVelocity_ReturnNoContent_WhenUpdateSuccessful()
        {
            // Arrange
            var shipId = Guid.NewGuid();
            var ship = new Ship { Id = shipId, Velocity = 5.0 };
            var newVelocity = 10.0;

            _shipServiceMock.Setup(service => service.GetByIdAsync(shipId)).ReturnsAsync(ship);
            _shipServiceMock.Setup(service => service.UpdateAsync(ship)).Returns(Task.CompletedTask);
            _responseHelperMock.Setup(helper => helper.CreateNoContentResponse()).Returns(new NoContentResult());

            // Act
            var result = await _controller.UpdateShipVelocity(shipId, newVelocity);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Equal(newVelocity, ship.Velocity);
        }
        #endregion

        #region GetShipsAssignedToUser
        [Fact]
        public async Task GetShipsAssignedToUser_ReturnOk_WithAssignedShips()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var ships = new List<Ship> { new Ship { Id = Guid.NewGuid(), Name = ConstantHelper.ShipTestName } };
            var shipDtos = new List<ShipDto> { new ShipDto { Id = ships[0].Id, Name = ConstantHelper.ShipTestName } };

            _shipServiceMock.Setup(service => service.GetShipsAssignedToUser(userId)).ReturnsAsync(ships);
            _mapperMock.Setup(mapper => mapper.Map<List<ShipDto>>(ships)).Returns(shipDtos);

            var userShipDto = new UserShipDto
            {
                UserId = userId,
                Ship = shipDtos
            };

            _responseHelperMock.Setup(helper => helper.CreateOkResponse(It.IsAny<UserShipDto>()))
                .Returns(new OkObjectResult(userShipDto));


            // Act
            var result = await _controller.GetShipsAssignedToUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<UserShipDto>(okResult.Value);

            Assert.Equal(userShipDto, returnedDto);
        }
        #endregion

        #region GetShipUnassigned
        [Fact]
        public async Task GetShipUnassigned_ReturnOk_WithUnassignedShips()
        {
            // Arrange
            var ships = new List<Ship> { new Ship { Id = Guid.NewGuid(), Name = ConstantHelper.ShipTestName } };
            var shipDtos = new List<ShipDto> { new ShipDto { Id = ships[0].Id, Name = ConstantHelper.ShipTestName } };

            _shipServiceMock.Setup(service => service.GetShipUnassigned()).ReturnsAsync(ships);
            _mapperMock.Setup(mapper => mapper.Map<List<ShipDto>>(ships)).Returns(shipDtos);
            _responseHelperMock.Setup(helper => helper.CreateOkResponse(shipDtos))
                .Returns(new OkObjectResult(shipDtos));

            // Act
            var result = await _controller.GetShipUnassigned();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(shipDtos, okResult.Value);
        }
        #endregion

        #region GetClosestPortToShip
        [Fact]
        public async Task GetClosestPortToShip_ReturnNotFound_WhenClosestPortIsNull()
        {
            // Arrange
            var shipId = Guid.NewGuid();
            _shipServiceMock.Setup(service => service.GetClosestPort(shipId)).ReturnsAsync((ClosestPortResultDto)null);
            _responseHelperMock.Setup(helper => helper.CreateNotFoundResponse(It.IsAny<string>()))
                .Returns(new NotFoundObjectResult(ConstantHelper.ItemNotFound));

            // Act
            var result = await _controller.GetClosestPortToShip(shipId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(ConstantHelper.ItemNotFound, notFoundResult.Value);
        }

        [Fact]
        public async Task GetClosestPortToShip_ReturnOk_WithClosestPort()
        {
            // Arrange
            var shipId = Guid.NewGuid();
            var closestPort = new ClosestPortResultDto
            {
                Ship = new ShipDto { Id = shipId, Name = ConstantHelper.ShipTestName },
                ClosestPort = new PortDto { Name = ConstantHelper.ShipTestName },
                ShipPortDistance = new DistanceDto { Distance = 100.90, Measure = "KM"},
                EstimationTime = new TimeDto { Hour = 1, Minute = 30, Second = 0 }
            };

            _shipServiceMock.Setup(service => service.GetClosestPort(shipId)).ReturnsAsync(closestPort);
            _responseHelperMock.Setup(helper => helper.CreateOkResponse(closestPort))
                .Returns(new OkObjectResult(closestPort));

            // Act
            var result = await _controller.GetClosestPortToShip(shipId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(closestPort, okResult.Value);
        }
        #endregion



    }
}
