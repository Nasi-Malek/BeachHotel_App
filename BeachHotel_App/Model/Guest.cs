using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeachHotel_App.Model
{
    public class Guest
    {
        [Key]
        public int GuestId { get; set; } // Primary Key

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; } 

        [MaxLength(15)]
        public string? ContactNumber { get; set; } 

        [MaxLength(100)]
        public string? Email { get; set; } 
        public List<Booking> Bookings { get; set; } = new List<Booking>(); 
    }
}


