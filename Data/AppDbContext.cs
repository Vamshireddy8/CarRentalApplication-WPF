using Microsoft.EntityFrameworkCore;
using CarRentalApplication.Models;

namespace CarRentalApplication.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; } 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=Car-Rental-DB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<Booking>().HasKey(b => b.Id);
            modelBuilder.Entity<Vehicle>().HasKey(v => v.Id);
        }
    }
}
