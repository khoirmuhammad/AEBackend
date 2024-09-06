using AEBackendProject.Models;

namespace AEBackendProject.Data
{
    public class PortSeeder
    {
        private readonly ApplicationDbContext _context;
        public PortSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public void SeedPorts()
        {
            var ports = new List<Port>
            {
                new Port { Id = Guid.NewGuid(), Name = "Tanjung Priok", Latitude = -6.1141, Longitude = 106.8456 },
                new Port { Id = Guid.NewGuid(), Name = "Tanjung Perak", Latitude = -7.2396, Longitude = 112.7354 },
                new Port { Id = Guid.NewGuid(), Name = "Belawan", Latitude = 3.7931, Longitude = 98.6837 },
                new Port { Id = Guid.NewGuid(), Name = "Makasar", Latitude = -5.1347, Longitude = 119.4328 },
                new Port { Id = Guid.NewGuid(), Name = "Batam", Latitude = -1.1211, Longitude = 104.0616 },
                new Port { Id = Guid.NewGuid(), Name = "Semarang", Latitude = -6.9667, Longitude = 110.4167 },
                new Port { Id = Guid.NewGuid(), Name = "Palembang", Latitude = -2.9167, Longitude = 104.7450 },
                new Port { Id = Guid.NewGuid(), Name = "Ambon", Latitude = -3.6545, Longitude = 128.1880 },
                new Port { Id = Guid.NewGuid(), Name = "Sorong", Latitude = -0.8833, Longitude = 131.2500 },
                new Port { Id = Guid.NewGuid(), Name = "Banjarmasin", Latitude = -3.3167, Longitude = 114.5833 }
            };

            foreach (var port in ports)
            {
                if (!_context.Ports.Any(p => p.Name == port.Name))
                {
                    _context.Ports.Add(port);
                }
            }

            _context.SaveChanges();
        }
    }
}
