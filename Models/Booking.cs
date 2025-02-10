using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalApplication.Models
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Vehicle Vehicle { get; set; }

        public User User { get; set; }
    
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int VehicleId { get; set; } 

        [Required]
        public string StartDate { get; set; }

        [Required]
        public string EndDate { get; set; }

        public string Status { get; set; } = "Pending";



    }
}
