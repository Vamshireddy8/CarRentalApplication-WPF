using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalApplication.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        [NotMapped]
        public string VehicleName => $"{Make} {Model} {Year}";

    }
}
