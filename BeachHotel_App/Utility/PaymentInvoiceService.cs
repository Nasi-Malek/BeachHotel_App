using BeachHotel_App.Data;
using BeachHotel_App.Model;
using BeachHotel_App.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeachHotel_App.Utility
{
    public class PaymentInvoiceService
    {
        private readonly HotelDbContext _context;

        public PaymentInvoiceService(HotelDbContext context)
        {
            _context = context;
        }

        public void AddInvoice(PaymentInvoice invoice)
        {
            try
            {
                _context.PaymentInvoices.Add(invoice);
                _context.SaveChanges();
                Console.WriteLine("Invoice added successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding invoice: {ex.Message}");
            }
        }

        public List<PaymentInvoice> GetAllInvoices()
        {
            var invoices = _context.PaymentInvoices.ToList();


            // Log details of each invoice
            foreach (var invoice in invoices)
            {
            }

            return invoices;
        }

        public PaymentInvoice GetInvoiceById(int id)
        {
            return _context.PaymentInvoices.FirstOrDefault(i => i.PaymentInvoiceId == id);
        }

        public void UpdateInvoice(int id, decimal newAmount, string newStatus, string newMethod, string newNotes)
        {
            try
            {
                var invoice = GetInvoiceById(id);
                if (invoice != null)
                {
                    invoice.Amount = newAmount;
                    invoice.PaymentStatus = newStatus;
                    invoice.PaymentMethod = newMethod;
                    invoice.InvoiceNotes = newNotes;

                    _context.SaveChanges();
                    Console.WriteLine("Invoice updated successfully!");
                }
                else
                {
                    Console.WriteLine("Invoice not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating invoice: {ex.Message}");
            }
        }

        public void DeleteInvoice(int id)
        {
            try
            {
                var invoice = GetInvoiceById(id);
                if (invoice != null)
                {
                    _context.PaymentInvoices.Remove(invoice);
                    _context.SaveChanges();
                    Console.WriteLine("Invoice deleted successfully!");
                }
                else
                {
                    Console.WriteLine("Invoice not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting invoice: {ex.Message}");
            }
        }
        public void CancelUnpaidBookings()
        {
            try
            {
                Console.WriteLine("Checking for unpaid bookings...");

                var unpaidBookings = _context.Bookings
                    .Where(b => !b.PaymentInvoices.Any(pi => pi.PaymentStatus == "Completed") &&
                                EF.Functions.DateDiffDay(b.CheckInDate, DateTime.Today) > 10)
                    .ToList();

                if (!unpaidBookings.Any())
                {
                    Console.WriteLine("No unpaid bookings found.");
                    return;
                }

                foreach (var booking in unpaidBookings)
                {
                    Console.WriteLine($"Canceling Booking ID: {booking.BookingId} due to non-payment.");
                    _context.Bookings.Remove(booking);
                }

                _context.SaveChanges();
                Console.WriteLine("Unpaid bookings canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during unpaid bookings cancellation: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }

    }

}
