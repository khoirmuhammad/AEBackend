using AEBackendProject.DTO.User;
using AEBackendProject.Models;
using AEBackendProject.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AEBackendProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public UserController(IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            var userDto = _mapper.Map<UserDto>(user);

            return user == null ? NotFound() : Ok(userDto);
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            var usersDto = _mapper.Map<List<UserDto>>(users);

            return Ok(usersDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto request)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var user = _mapper.Map<User>(request);

            await _userService.CreateAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpPost("{userId}/assign/{shipId}")]
        public async Task<IActionResult> AssignShipToUser(Guid userId, Guid shipId)
        {
            await _userService.AssignShipToUser(userId, shipId);
            return NoContent();
        }

        [HttpPost("{userId}/unassign/{shipId}")]
        public async Task<IActionResult> UnAssignShipToUser(Guid userId, Guid shipId)
        {
            await _userService.UnAssignShipToUser(userId, shipId);
            return NoContent();
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
