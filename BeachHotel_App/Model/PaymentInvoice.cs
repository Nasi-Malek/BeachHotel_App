using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeachHotel_App.Model
{
    public class PaymentInvoice
    {
        [Key]
        public int PaymentInvoiceId { get; set; } // Primary Key

        [ForeignKey(nameof(Booking))]
        public int BookingId { get; set; } // Foreign Key to Booking

        [Required]
        public decimal Amount { get; set; } 

        [Required]
        public DateTime? PaymentDate { get; set; } 

        [Required]
        [MaxLength(50)]
        public string? PaymentMethod { get; set; } 

        [MaxLength(20)]
        public string? PaymentStatus { get; set; } 

        [Required]
        public DateTime InvoiceDate { get; set; } 

        [MaxLength(500)]
        public string? InvoiceNotes { get; set; } 

        // Navigation Property
        public Booking? Booking { get; set; } 

    }
}

