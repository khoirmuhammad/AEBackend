using AEBackendProject.Common;
using AEBackendProject.DTO.Port;
using AEBackendProject.Models;

namespace AEBackendProject.Services
{
    public class ConverterInKm : IConverter
    {
        public double GetSpeed(double speedInKnots)
        {
            return speedInKnots * Constant.KnotInKM;
        }

        //Haversine Formula
        public double GetDistance(double shipLat, double shipLon, double portLat, double portLon)
        {
            // Convert Latitude and Longitude to Radians: Latitude and longitude values need to be converted from degrees to radians before using them in the formula
            double latShipRad = GetRadians(shipLat);
            double lonShipRad = GetRadians(shipLon);
            double latPortRad = GetRadians(portLat);
            double lonPortRad = GetRadians(portLon);

            // Calculate the Differences: Compute the differences in latitude and longitude between the two points
            double latitudeDifference = latPortRad - latShipRad;
            double longitudeDifference = lonPortRad - lonShipRad;

            double centralAnglePart = (Math.Pow(Math.Sin(latitudeDifference / 2), 2)) +
                                      ((Math.Cos(latShipRad) * Math.Cos(latPortRad)) * Math.Pow(Math.Sin(longitudeDifference / 2), 2));

            double angularDistance = 2 * Math.Atan2(Math.Sqrt(centralAnglePart), Math.Sqrt(1 - centralAnglePart));

            // Return the distance in kilometers
            return Constant.EarthRadiusKm * angularDistance;
        }

        public double GetRadians(double degree)
        {
            return degree * (Math.PI / 180.0);
        }

        public DistanceDto GetDistanceDto(double distance)
        {
            var result = new DistanceDto
            {
                Distance = Math.Round(distance, 2),
                Measure = "KM"
            };

            return result;
        }
    }
}
