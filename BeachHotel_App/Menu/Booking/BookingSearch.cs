using BeachHotel_App.Data;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace BeachHotel_App.Menu.Booking_Menu
{
    public class BookingSearch
    {
        public void SearchAvailableRooms()
        {
            try
            {
                AnsiConsole.Clear();
                AnsiConsole.Markup("[bold yellow]Search for Available Rooms[/]\n");

                // Ask for search criteria
                var checkInDate = AnsiConsole.Ask<DateTime>("Enter [yellow]Check-In Date (yyyy-MM-dd)[/]:");
                var checkOutDate = AnsiConsole.Ask<DateTime>("Enter [yellow]Check-Out Date (yyyy-MM-dd)[/]:");
                var numberOfPeople = AnsiConsole.Ask<int>("Enter [yellow]Number of People[/]:");

                if (checkOutDate <= checkInDate)
                {
                    AnsiConsole.Markup("[red]Error: Check-Out Date must be after Check-In Date.[/]\n");
                    return;
                }

                using (var context = new HotelDbContext())
                {
                    // Query to find available rooms
                    var availableRooms = context.Rooms
                        .Where(r => r.MaxGuests >= numberOfPeople && !context.Bookings
                            .Any(b => b.RoomNumber == r.RoomNumber &&
                                      ((checkInDate >= b.CheckInDate && checkInDate < b.CheckOutDate) ||
                                       (checkOutDate > b.CheckInDate && checkOutDate <= b.CheckOutDate))))
                        .ToList();

                    if (!availableRooms.Any())
                    {
                        AnsiConsole.Markup("[red]No rooms available for the specified criteria.[/]\n");
                        return;
                    }

                    // Display available rooms
                    var table = new Table();
                    table.AddColumn("[cyan]Room Number[/]");
                    table.AddColumn("[cyan]Max Guests[/]");
                    table.AddColumn("[cyan]Price[/]");

                    foreach (var room in availableRooms)
                    {
                        table.AddRow(room.RoomNumber.ToString(), room.MaxGuests.ToString(), $"{room.Price:C}");
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
