namespace AEBackendProject.DTO.Ship
{
    public class UserShipDto
    {
        public Guid UserId { get; set; }
        public List<ShipDto> Ship { get; set; }
    }
}
