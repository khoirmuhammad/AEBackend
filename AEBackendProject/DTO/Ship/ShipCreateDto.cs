namespace AEBackendProject.DTO.Ship
{
    public class ShipCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public double Latitude {  get; set; }
        public double Longitude { get; set; }
        public double Velocity { get; set; }
    }
}
