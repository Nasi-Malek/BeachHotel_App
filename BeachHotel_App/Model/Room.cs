using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeachHotel_App.Model
{
    public class Room
    {
        [Key]
        public int RoomNumber { get; set; }// Primary Key

        [Required]
        [MaxLength(50)]
        public string? Type { get; set; } 

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; } 

        public bool IsAvailable { get; set; } 
        [Range(0, int.MaxValue, ErrorMessage = "Extra beds must be non-negative.")]
        public int ExtraBeds { get; set; } 

        [Range(1, int.MaxValue, ErrorMessage = "Size must be a positive value.")]
        public int Size { get; set; } 

        [Range(1, int.MaxValue, ErrorMessage = "Maximum guests must be at least 1.")]
        public int MaxGuests { get; set; } 
        public List<Booking> Bookings { get; set; } = new List<Booking>(); 
    }
}