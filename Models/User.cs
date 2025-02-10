using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalApplication.Models 
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } 

        [Required]
        public string Password { get; set; }

        public ICollection<Booking> Bookings { get; set; } = [];

        [Required]
        public string Role { get; set; } = "User"; 
    }
}
