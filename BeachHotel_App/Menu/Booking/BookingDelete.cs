using BeachHotel_App.Data;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;


namespace BeachHotel_App.Menu.Booking_Menu
{
    public class BookingDelete
    {
        public void CancelBooking()
        {

            try
            {
                AnsiConsole.Clear();
                AnsiConsole.Markup("[bold red]Cancel a Booking[/]\n");

                var guestId = AnsiConsole.Ask<int>("Enter [yellow]Guest ID[/] associated with the booking:");

                using (var context = new HotelDbContext())
                {

                    var activeBookings = context.Bookings.Where(b => b.GuestId == guestId).ToList();

                    if (!activeBookings.Any())
                    {
                        AnsiConsole.Markup("[red]No active bookings found for this guest.[/]\n");
                        return;
                    }


                    var bookingTable = new Table()
                        .AddColumn("Booking ID")
                        .AddColumn("Room Number")
                        .AddColumn("Check-In Date")
                        .AddColumn("Check-Out Date");

                    foreach (var booking in activeBookings)
                    {
                        bookingTable.AddRow(
                            booking.BookingId.ToString(),
                            booking.RoomNumber.ToString(),
                            booking.CheckInDate.ToShortDateString(),
                            booking.CheckOutDate.ToShortDateString()
                        );
                    }

                    AnsiConsole.Write(bookingTable);

                    // Ask for the specific booking ID to cancel
                    var bookingId = AnsiConsole.Ask<int>("Enter the [yellow]Booking ID[/] to cancel:");

                    var bookingToCancel = activeBookings.FirstOrDefault(b => b.BookingId == bookingId);

                    if (bookingToCancel == null)
                    {
                        AnsiConsole.Markup("[red]Booking not found for this guest.[/]\n");
                        return;
                    }

                    // Confirm cancellation
                    if (AnsiConsole.Confirm($"Are you sure you want to cancel Booking ID [red]{bookingId}[/]?"))
                    {
                        context.Bookings.Remove(bookingToCancel);
                        context.SaveChanges();
                        AnsiConsole.Markup("[green]Booking cancelled successfully![/]\n");
                    }
                    else
                    {
                        AnsiConsole.Markup("[yellow]Operation cancelled.[/]\n");
                    }
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup($"[red]Error: {ex.Message}[/]\n");
            }
        }
    }
}



