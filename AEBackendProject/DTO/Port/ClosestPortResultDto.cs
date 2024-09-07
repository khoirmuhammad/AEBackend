using AEBackendProject.DTO.Ship;

namespace AEBackendProject.DTO.Port
{
    public class ClosestPortResultDto
    {
        public ShipDto Ship { get; set; }
        public PortDto ClosestPort {  get; set; }
        public DistanceDto ShipPortDistance { get; set; }
        public TimeDto EstimationTime { get; set; }
    }

    public class PortDto
    {
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class DistanceDto
    {
        public double Kilometer { get; set; }
        public double Meter { get; set; }
    }

    public class TimeDto
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
    }

}
