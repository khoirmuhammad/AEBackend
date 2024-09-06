using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AEBackendProject.DTO.Ship
{
    public class ShipDto
    {
        public Guid Id { get; set; } 

        public string Name { get; set; } = string.Empty;

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public double Velocity { get; set; }
    }
}
