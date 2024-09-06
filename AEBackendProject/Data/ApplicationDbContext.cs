using AEBackendProject.Models;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AEBackendProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Ship> Ships { get; set; }
        public DbSet<Port> Ports { get; set; }
        public DbSet<UserShip> UserShips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Ship>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Port>().HasQueryFilter(e => !e.IsDeleted);

            // Configuring many - to - many relationship
            modelBuilder.Entity<UserShip>().HasKey(us => new { us.UserId, us.ShipId });

            modelBuilder.Entity<UserShip>()
                .HasOne(us => us.User)
                .WithMany(u => u.UserShips)
                .HasForeignKey(us => us.UserId);

            modelBuilder.Entity<UserShip>()
                .HasOne(us => us.Ship)
                .WithMany(s => s.UserShips)
                .HasForeignKey(us => us.ShipId);
        }
    }
}
