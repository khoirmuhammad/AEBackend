using AEBackendProject.DTO.Ship;
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
    public class ShipController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IShipService _shipService;
        public ShipController(IMapper mapper, IShipService shipService)
        {
            _mapper = mapper;
            _shipService = shipService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShipById(Guid id)
        {
            var user = await _shipService.GetByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpGet("GetAllShips")]
        public async Task<IActionResult> GetAllShips()
        {
            var users = await _shipService.GetAllAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShip([FromBody] ShipCreateDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ship = _mapper.Map<Ship>(request);

            await _shipService.CreateAsync(ship);
            return CreatedAtAction(nameof(GetShipById), new { id = ship.Id }, ship);
        }

        [HttpPut("{id}/velocity")]
        public async Task<IActionResult> UpdateShipVelocity(Guid id, [FromBody] double newVelocity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ship = await _shipService.GetByIdAsync(id);
            if (ship == null)
                return NotFound($"ship id {id} not found");

            ship.Velocity = newVelocity;

            await _shipService.UpdateAsync(ship);
            return NoContent();
        }

        [HttpGet("assigned/{userId}")]
        public async Task<IActionResult> GetShipsAssignedToUser(Guid userId)
        {
            var ships = await _shipService.GetShipsAssignedToUser(userId);

            var shipDto = _mapper.Map<List<ShipDto>>(ships);

            var userShipDto = new UserShipDto
            {
                UserId = userId,
                Ship = shipDto
            };

            return Ok(userShipDto);
        }

        [HttpGet("unassigned")]
        public async Task<IActionResult> GetShipUnassigned()
        {
            var ships = await _shipService.GetShipUnassigned();

            var shipDto = _mapper.Map<List<ShipDto>>(ships);

            return Ok(shipDto);
        }

        [HttpGet("closest-port")]
        public async Task<IActionResult> GetClosestPortToShip(Guid shipId)
        {
            var closestPort = await _shipService.GetClosestPort(shipId);
            return Ok(closestPort);
        }
    }
}
