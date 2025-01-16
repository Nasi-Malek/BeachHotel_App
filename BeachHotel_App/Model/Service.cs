using BeachHotel_App.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeachHotel_App.Model
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; } // Primary Key

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; } 

        [Required]
        public decimal Price { get; set; } 

        [MaxLength(500)]
        public string? Description { get; set; } 

        // navigation property for the many-to-many relationship
        public List<BookingService> BookingServices { get; set; } = new List<BookingService>();
    }
}

