using AEBackendProject.Common;
using AEBackendProject.Controllers;
using AEBackendProject.DTO.User;
using AEBackendProject.Models;
using AEBackendProject.Services;
using AEBackendProject.Test.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AEBackendProject.Test.Controllers
{
    public class UserControllerTest
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IResponseHelper> _mockResponseHelper;
        private readonly UserController _controller;

        public UserControllerTest()
        {
            _mockMapper = new Mock<IMapper>();
            _mockUserService = new Mock<IUserService>();
            _mockResponseHelper = new Mock<IResponseHelper>();
            _controller = new UserController(_mockMapper.Object, _mockUserService.Object, _mockResponseHelper.Object);
        }

        #region GetUserById
        [Fact]
        public async Task GetUserById_ReturnNotFound_WhenUserIsNull()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockUserService.Setup(service =>
                service.GetByIdAsync(userId)).ReturnsAsync((User)null);
            _mockResponseHelper.Setup(helper => 
                helper.CreateNotFoundResponse(It.IsAny<string>())).Returns(new NotFoundObjectResult(ConstantHelper.UserNotFound));

            // Act
            var user = await _controller.GetUserById(userId);

            // Assert
            var result = Assert.IsType<NotFoundObjectResult>(user);
            Assert.Equal(ConstantHelper.UserNotFound, result.Value);
        }

        [Fact]
        public async Task GetUserById_ReturnOk_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userMock = new User { Id = userId };
            var userDto = new UserDto();
            _mockUserService.Setup(service => service.GetByIdAsync(userId)).ReturnsAsync(userMock);
            _mockMapper.Setup(mapper => mapper.Map<UserDto>(userMock)).Returns(userDto);
            _mockResponseHelper.Setup(helper => helper.CreateOkResponse(userDto)).Returns(new OkObjectResult(userDto));

            // Act
            var user = await _controller.GetUserById(userId);

            // Assert
            var result = Assert.IsType<OkObjectResult>(user);
            Assert.Equal(userDto, result.Value);
        }
        #endregion

        #region GetAllUsers
        [Fact]
        public async Task GetAllUsers_ReturnOk_WithUsers()
        {
            // Arrange
            var usersMock = new List<User> { new User() };
            var userDtos = new List<UserDto> { new UserDto() };
            _mockUserService.Setup(service => service.GetAllAsync()).ReturnsAsync(usersMock);
            _mockMapper.Setup(mapper => mapper.Map<List<UserDto>>(usersMock)).Returns(userDtos);
            _mockResponseHelper.Setup(helper => helper.CreateOkResponse(userDtos)).Returns(new OkObjectResult(userDtos));

            // Act
            var users = await _controller.GetAllUsers();

            // Assert
            var result = Assert.IsType<OkObjectResult>(users);
            Assert.Equal(userDtos, result.Value);
        }
        #endregion

        #region CreateUser
        [Fact]
        public async Task CreateUser_ReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var request = new UserCreateDto();
            _controller.ModelState.AddModelError("error", ConstantHelper.UserCreateBadRequest);
            _mockResponseHelper.Setup(helper => helper.CreateBadRequestResponse(It.IsAny<IEnumerable<string>>())).Returns(new BadRequestObjectResult("error"));

            // Act
            var actionResult = await _controller.CreateUser(request);

            // Assert
            var result = Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.Equal("error", result.Value);
        }

        [Fact]
        public async Task CreateUser_ReturnCreatedResponse_WhenModelStateIsValid()
        {
            // Arrange
            var request = new UserCreateDto();
            var user = new User();
            var userId = Guid.NewGuid();
            user.Id = userId;
            _mockMapper.Setup(mapper => mapper.Map<User>(request)).Returns(user);
            _mockUserService.Setup(service => service.CreateAsync(user)).Returns(Task.CompletedTask);
            _mockResponseHelper.Setup(helper => helper.CreateCreatedResponse(user, userId, nameof(UserController), nameof(UserController.GetUserById)))
                .Returns(new CreatedAtActionResult(nameof(UserController.GetUserById), nameof(UserController), new { id = userId }, user));

            // Act
            var actionResult = await _controller.CreateUser(request);

            // Assert
            var result = Assert.IsType<CreatedAtActionResult>(actionResult);
            Assert.Equal(user, result.Value);
        }
        #endregion

        #region AssignShipToUser
        [Fact]
        public async Task AssignShipToUser_ReturnsCreatedResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var shipId = Guid.NewGuid();
            _mockUserService.Setup(service => service.AssignShipToUser(userId, shipId)).Returns(Task.CompletedTask);
            _mockResponseHelper.Setup(helper => helper.CreateCustomResponse((int)HttpStatusCode.Created, "Ship successfully assigned to user."))
                .Returns(new StatusCodeResult((int)HttpStatusCode.Created));

            // Act
            var actionResult = await _controller.AssignShipToUser(userId, shipId);

            // Assert
            var result = Assert.IsType<StatusCodeResult>(actionResult);
            Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
        }
        #endregion

        #region UnAssignShipToUser
        [Fact]
        public async Task UnAssignShipToUser_ReturnCreatedResponse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var shipId = Guid.NewGuid();
            _mockUserService.Setup(service => service.UnAssignShipToUser(userId, shipId)).Returns(Task.CompletedTask);
            _mockResponseHelper.Setup(helper => helper.CreateCustomResponse((int)HttpStatusCode.Created, "Ship successfully unassigned to user."))
                .Returns(new StatusCodeResult((int)HttpStatusCode.Created));

            // Act
            var actionResult = await _controller.UnAssignShipToUser(userId, shipId);

            // Assert
            var result = Assert.IsType<StatusCodeResult>(actionResult);
            Assert.Equal((int)HttpStatusCode.Created, result.StatusCode);
        }
        #endregion

    }

}
