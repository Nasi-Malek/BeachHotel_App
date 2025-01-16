using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeachHotel_App.Services;

namespace BeachHotel_App.Model
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [ForeignKey(nameof(Room))]
        public int RoomNumber { get; set; }

        [ForeignKey(nameof(Guest))]
        public int GuestId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        public Room? Room { get; set; }
        public Guest? Guest { get; set; }


        public List<BookingService> BookingServices { get; set; } = new List<BookingService>();
        // Navigation Property for related invoices
        public ICollection<PaymentInvoice>? PaymentInvoices { get; set; }
    }
}

