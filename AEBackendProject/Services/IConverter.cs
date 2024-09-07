using AEBackendProject.DTO.Port;

namespace AEBackendProject.Services
{
    public interface IConverter
    {
        double GetSpeed(double speedInKnots);
        double GetDistance(double shipLat, double shipLon, double portLat, double portLon);
        double GetRadians(double degree);
        DistanceDto GetDistanceDto(double distance);
    }
}
