namespace AEBackendProject.DTO.Port
{
    public class PortDistanceTimeDto
    {
        public string PortName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Distance { get; set; }
        public double Time { get; set; }
    }
}
