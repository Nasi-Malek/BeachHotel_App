using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BeachHotel_App.Data;
using BeachHotel_App.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BeachHotel_App.Services
{
    public class BookingService
    {
        [Key]
        public int BookingServiceId { get; set; } // Primary Key

        [ForeignKey(nameof(Booking))]
        public int BookingId { get; set; } // Foreign Key to Booking

        [ForeignKey(nameof(Service))]
        public int ServiceId { get; set; } // Foreign Key to Service

        public int Quantity { get; set; } 
        public string? ServiceName { get; set; } 

        public Booking? Booking { get; set; } 
        public Service? Service { get; set; } 
    }
}
