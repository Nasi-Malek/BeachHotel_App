using BeachHotel_App.Data;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace BeachHotel_App.Menu.Booking_Menu
{
    public class BookingView
    {
        public void ViewAllBookings()
        {
            try
            {
                AnsiConsole.Clear();
                AnsiConsole.Markup("[bold yellow]View All Bookings[/]\n");

                using (var context = new HotelDbContext())
                {
                    var bookings = context.Bookings
                        .Include(b => b.Room)
                        .Include(b => b.Guest)
                        .ToList();

                    if (!bookings.Any())
                    {
                        AnsiConsole.Markup("[red]No bookings found.[/]\n");
                        return;
                    }

                    var table = new Table();
                    table.AddColumn("[cyan]Booking ID[/]");
                    table.AddColumn("[cyan]Guest ID[/]");
                    table.AddColumn("[cyan]Room ID[/]");
                    table.AddColumn("[cyan]Guest Name[/]");
                    table.AddColumn("[cyan]Check-In[/]");
                    table.AddColumn("[cyan]Check-Out[/]");

                    foreach (var booking in bookings)
                    {
                        table.AddRow(
                            booking.BookingId.ToString(),
                            booking.GuestId.ToString(),
                            booking.Room.RoomNumber.ToString(),
                            booking.Guest.Name,
                            booking.CheckInDate.ToShortDateString(),
                            booking.CheckOutDate.ToShortDateString());
                    }

                    AnsiConsole.Write(table);
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup($"[red]Error: {ex.Message}[/]\n");
            }
        }
    }
}