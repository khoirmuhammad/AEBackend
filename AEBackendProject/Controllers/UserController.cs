using AEBackendProject.Common;
using AEBackendProject.Common.Validators;
using AEBackendProject.DTO.User;
using AEBackendProject.Models;
using AEBackendProject.Services;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace AEBackendProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IResponseHelper _responseHelper;
        public UserController(IMapper mapper, IUserService userService, IResponseHelper responseHelper)
        {
            _mapper = mapper;
            _userService = userService;
            _responseHelper = responseHelper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById([FromRoute, Required, NotEmptyGuid] Guid id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return _responseHelper.CreateNotFoundResponse("User not found");

            var userDto = _mapper.Map<UserDto>(user);
            return _responseHelper.CreateOkResponse(userDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            var usersDto = _mapper.Map<List<UserDto>>(users);

            return _responseHelper.CreateOkResponse(usersDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto request)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return _responseHelper.CreateBadRequestResponse(errors);
            }

            var user = _mapper.Map<User>(request);
            await _userService.CreateAsync(user);

            return _responseHelper.CreateCreatedResponse(user, user.Id, nameof(UserController), nameof(GetUserById));
        }

        [HttpPost("{userId}/assign/{shipId}")]
        public async Task<IActionResult> AssignShipToUser([FromRoute, Required, NotEmptyGuid] Guid userId, [FromRoute, Required, NotEmptyGuid] Guid shipId)
        {
            await _userService.AssignShipToUser(userId, shipId);
            return _responseHelper.CreateCustomResponse((int)HttpStatusCode.Created, "Ship successfully assigned to user.");
        }

        [HttpPost("{userId}/unassign/{shipId}")]
        public async Task<IActionResult> UnAssignShipToUser([FromRoute, Required, NotEmptyGuid] Guid userId, [FromRoute, Required, NotEmptyGuid] Guid shipId)
        {
            await _userService.UnAssignShipToUser(userId, shipId);
            return _responseHelper.CreateCustomResponse((int)HttpStatusCode.Created, "Ship successfully unassigned to user.");
        }

        //[HttpGet("GetUsers")]
        //public async Task<IActionResult> GetUsers([FromQuery] string nameFilter, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        //{
        //    var users = await _userService.GetAsync(nameFilter, pageNumber, pageSize);
        //    return Ok(users);
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserCreateDto request)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var user = await _userService.GetByIdAsync(id);
        //    if (user == null)
        //        return NotFound($"user id {id} not found");

        //    user.Name = request.Name;
        //    user.Role = request.Role;

        //    await _userService.UpdateAsync(user);
        //    return NoContent();
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteUser(Guid id)
        //{
        //    var user = await _userService.GetByIdAsync(id);
        //    if (user == null)
        //        return NotFound($"user id {id} not found");

        //    user.IsDeleted = true;

        //    await _userService.UpdateAsync(user);
        //    return NoContent();
        //}

    }
}
