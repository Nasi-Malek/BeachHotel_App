using BeachHotel_App.Data;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeachHotel_App.Menu.Booking_Menu
{
    public class BookingUpdate
    {
        public void UpdateBooking()
        {
            try
            {
                AnsiConsole.Clear();
                AnsiConsole.Markup("[bold yellow]Update a Booking[/]\n");

                // Logic to update booking
                AnsiConsole.Markup("[green]Booking updated successfully![/]\n");
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup($"[red]Error: {ex.Message}[/]\n");
            }
        }
    }
}
