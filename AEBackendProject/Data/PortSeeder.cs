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
            try
            {
                // List of ports to seed
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

                // Fetch existing ports from the database
                var existingPortNames = _context.Ports
                    .Select(p => p.Name)
                    .ToHashSet(); // Using HashSet for O(1) lookups

                var newPorts = ports
                    .Where(port => !existingPortNames.Contains(port.Name))
                    .ToList();

                if (newPorts.Any())
                {
                    _context.Ports.AddRange(newPorts);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while seeding ports: {ex.Message}");
                throw;
            }
        }
    }
}
