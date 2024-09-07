using AEBackendProject.Common;
using AEBackendProject.Common.Validators;
using AEBackendProject.DTO.Port;
using AEBackendProject.DTO.Ship;
using AEBackendProject.DTO.User;
using AEBackendProject.Models;
using AEBackendProject.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AEBackendProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IShipService _shipService;
        private readonly IResponseHelper _responseHelper;
        public ShipController(IMapper mapper, IShipService shipService, IResponseHelper responseHelper)
        {
            _mapper = mapper;
            _shipService = shipService;
            _responseHelper = responseHelper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShipById([FromRoute, Required, NotEmptyGuid] Guid id)
        {
            var ship = await _shipService.GetByIdAsync(id);
            
            if (ship == null)
                return _responseHelper.CreateNotFoundResponse("Item not found");

            var shipDto = _mapper.Map<ShipDto>(ship);

            return _responseHelper.CreateOkResponse(shipDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShips()
        {
            var ships = await _shipService.GetAllAsync();
            var shipsDto = _mapper.Map<List<ShipDto>>(ships);

            return _responseHelper.CreateOkResponse(shipsDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShip([FromBody] ShipCreateDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return _responseHelper.CreateBadRequestResponse(errors);
            }

            var ship = _mapper.Map<Ship>(request);
            await _shipService.CreateAsync(ship);

            return _responseHelper.CreateCreatedResponse(ship, ship.Id, nameof(ShipController), nameof(GetShipById));
        }

        [HttpPatch("{id}/velocity")]
        public async Task<IActionResult> UpdateShipVelocity([FromRoute, Required, NotEmptyGuid] Guid id, [FromBody] double newVelocity)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return _responseHelper.CreateBadRequestResponse(errors);
            }

            var ship = await _shipService.GetByIdAsync(id);
            if (ship == null)
                return _responseHelper.CreateNotFoundResponse("Item not found");

            ship.Velocity = newVelocity;

            await _shipService.UpdateAsync(ship);
            return _responseHelper.CreateNoContentResponse();
        }

        [HttpGet("assigned/{userId}")]
        public async Task<IActionResult> GetShipsAssignedToUser([FromRoute, Required, NotEmptyGuid] Guid userId)
        {
            var ships = await _shipService.GetShipsAssignedToUser(userId);
            var shipDto = _mapper.Map<List<ShipDto>>(ships);

            var userShipDto = new UserShipDto
            {
                UserId = userId,
                Ship = shipDto
            };

            return _responseHelper.CreateOkResponse(userShipDto);
        }

        [HttpGet("unassigned")]
        public async Task<IActionResult> GetShipUnassigned()
        {
            var ships = await _shipService.GetShipUnassigned();
            var shipDto = _mapper.Map<List<ShipDto>>(ships);

            return _responseHelper.CreateOkResponse(shipDto);
        }

        [HttpGet("closest-port")]
        public async Task<IActionResult> GetClosestPortToShip([FromRoute, Required, NotEmptyGuid] Guid shipId)
        {
            var closestPort = await _shipService.GetClosestPort(shipId);
            if (closestPort == null)
                return _responseHelper.CreateNotFoundResponse("Item not found");

            return _responseHelper.CreateOkResponse(closestPort);
        }
    }
}
