using BeachHotel_App.Data;
using BeachHotel_App.Model;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeachHotel_App.Menu.Booking_Menu
{
    public class BookingAdd
    {
        public void CreateBooking()
        {
            try
            {
                AnsiConsole.Clear();
                AnsiConsole.Markup("[bold yellow]Create a Booking[/]\n");

                var guestId = AnsiConsole.Ask<int>("Enter [yellow]Guest ID[/]:");
                using (var context = new HotelDbContext())
                {
                    if (!context.Guests.Any(g => g.GuestId == guestId))
                    {
                        AnsiConsole.Markup("[red]Error: Guest with ID [/][yellow]{guestId}[/][red] does not exist.[/]\n");
                        return;
                    }
                }
                var roomNumber = AnsiConsole.Ask<int>("Enter [yellow]Room Number[/]:");
                using (var context = new HotelDbContext())
                {
                    if (!context.Rooms.Any(r => r.RoomNumber == roomNumber))
                    {
                        AnsiConsole.Markup("[red]Error: Room number [/][yellow]{roomNumber}[/][red] does not exist.[/]\n");
                        return;
                    }
                }
                var checkInDate = SelectDate("Select [yellow]Check-In Date[/]:");
                var checkOutDate = SelectDate("Select [yellow]Check-Out Date[/]:");

                if (checkOutDate <= checkInDate)
                {
                    AnsiConsole.Markup("[red]Error: Check-Out Date must be after Check-In Date.[/]\n");
                    return;
                }


                using (var context = new HotelDbContext())
                {
                    // Validate GuestId
                    if (!context.Guests.Any(g => g.GuestId == guestId))
                    {
                        AnsiConsole.Markup("[red]Error: Guest does not exist.[/]\n");
                        return;
                    }

                    // Validate RoomNumber
                    if (!context.Rooms.Any(r => r.RoomNumber == roomNumber))
                    {
                        AnsiConsole.Markup("[red]Error: Room does not exist.[/]\n");
                        return;
                    }

                    // Check if the room is already booked
                    bool roomIsBooked = context.Bookings.Any(b =>
                        b.RoomNumber == roomNumber &&
                        ((checkInDate >= b.CheckInDate && checkInDate < b.CheckOutDate) ||
                         (checkOutDate > b.CheckInDate && checkOutDate <= b.CheckOutDate)));

                    if (roomIsBooked)
                    {
                        AnsiConsole.Markup("[red]Error: The room is already booked for the selected dates.[/]\n");
                        return;
                    }

                    // Create the booking
                    var booking = new Booking
                    {
                        GuestId = guestId,
                        RoomNumber = roomNumber,
                        CheckInDate = checkInDate,
                        CheckOutDate = checkOutDate,
                    };

                    context.Bookings.Add(booking);

                    // Save changes with exception handling
                    try
                    {
                        context.SaveChanges();
                        AnsiConsole.Markup("[green]Booking created successfully![/]\n");
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.Markup("[red]An error occurred while saving the booking.[/]\n");
                        Console.WriteLine("Error: " + ex.Message);
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                        }
                        throw;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                AnsiConsole.Markup("[yellow]Booking creation canceled by the user.[/]\n");
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup($"[red]Error: {ex.Message}[/]\n");
            }
        }


        // Method to display and navigate calendar
        private DateTime SelectDate(string prompt)
        {
            // Start date (beginning of the month)
            DateTime currentDate = DateTime.Now;
            DateTime selectedDate = new DateTime(currentDate.Year, currentDate.Month, 1);

            while (true)
            {
                Console.Clear();
                RenderCalendar(selectedDate);

                // Read the user's key
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.RightArrow:
                        selectedDate = selectedDate.AddDays(1);
                        break;
                    case ConsoleKey.LeftArrow:
                        selectedDate = selectedDate.AddDays(-1);
                        break;
                    case ConsoleKey.UpArrow:
                        selectedDate = selectedDate.AddDays(-7);
                        break;
                    case ConsoleKey.DownArrow:
                        selectedDate = selectedDate.AddDays(7);
                        break;
                    case ConsoleKey.Enter:
                        AnsiConsole.MarkupLine($"\nYou selected: [green]{selectedDate:yyyy-MM-dd}[/]");
                        return selectedDate; 
                    case ConsoleKey.Escape:
                        throw new OperationCanceledException("Date selection canceled."); // Exit with exception if canceled
                }
            }
        }

        static void RenderCalendar(DateTime selectedDate)
        {
            var calendarContent = new StringWriter();

            // Calendar header
            calendarContent.WriteLine($"[red]{selectedDate:MMMM}[/]".ToUpper());
            calendarContent.WriteLine("Mon  Tue  Wed  Thu  Fri  Sat  Sun");
            calendarContent.WriteLine("─────────────────────────────────");

            DateTime firstDayOfMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);
            int startDay = (int)firstDayOfMonth.DayOfWeek;
            startDay = (startDay == 0) ? 6 : startDay - 1; // Adjust for Monday as the week start

            // Fill with empty spaces before the first day of the month
            for (int i = 0; i < startDay; i++)
            {
                calendarContent.Write("     ");
            }

            // Print the days
            for (int day = 1; day <= daysInMonth; day++)
            {
                if (day == selectedDate.Day)
                {
                    
                    calendarContent.Write($"[green]{day,2}[/]   ");
                }
                else
                {
                    calendarContent.Write($"{day,2}   ");
                }

                // Move to the next row after Sunday
                if ((startDay + day) % 7 == 0)
                {
                    calendarContent.WriteLine();
                }
            }

            // Create a panel with double borders
            var panel = new Panel(calendarContent.ToString())
            {
                Border = BoxBorder.Double,
                Header = new PanelHeader($"[red]{selectedDate:yyyy}[/]", Justify.Center)
            };

            AnsiConsole.Write(panel);
            Console.WriteLine();
            AnsiConsole.MarkupLine("\nUse the arrow keys [blue]\u25C4 \u25B2 \u25BA \u25BC[/] to \nnavigate and [green]Enter[/] to select.");
        }

    }
}