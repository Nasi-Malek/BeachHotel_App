using BeachHotel_App.Data;
using BeachHotel_App.Model;
using BeachHotel_App.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using System;
using System.Linq;

namespace BeachHotel_App.Menu
{
    public class RoomMenu
    {
        private readonly RoomService dbHelper;
        private readonly Validator validator;

        public RoomMenu()
        {
            var configuration = new ConfigurationBuilder()
         .SetBasePath(Directory.GetCurrentDirectory())
         .AddJsonFile("appsettings.json")
         .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<HotelDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            dbHelper = new RoomService(optionsBuilder.Options);
            validator = new Validator();
        }

        public void DisplayRoomMenu()
        {
            while (true)
            {
                AnsiConsole.Write(new FigletText("Room Menu").Centered().Color(Spectre.Console.Color.Green));

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Select an option:[/]")
                        .AddChoices(new[]
                        {
                            "View All Rooms",
                            "Create a New Room",
                            "Edit a Room",
                            "Delete a Room",
                            "Search Available Rooms",
                            "Back to Main Menu"
                        }));

                switch (choice)
                {
                    case "View All Rooms":
                        ViewAllRooms();
                        break;
                    case "Create a New Room":
                        CreateRoom();
                        break;
                    case "Edit a Room":
                        EditRoom();
                        break;
                    case "Delete a Room":
                        DeleteRoom();
                        break;
                    case "Search Available Rooms":
                        SearchAvailableRooms();
                        break;
                    case "Back to Main Menu":
                        return;
                }

                AnsiConsole.Markup("\n[green]Press any key to return to the menu...[/]");
                Console.ReadKey();
            }
        }

        private void CreateRoom()
        {
            try
            {
                AnsiConsole.Clear();
                AnsiConsole.Markup("[bold yellow]Create a New Room[/]\n");

                // Use SelectionPrompt to select room type
                var type = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select [yellow]Room Type[/]:")
                        .AddChoices(new[] { "Single", "Double", "Suite", }));


                var price = AnsiConsole.Ask<decimal>("Enter [yellow]Room Price[/]:");
                var extraBeds = AnsiConsole.Ask<int>("Enter [yellow]Extra Beds[/]:");
                var maxGuests = AnsiConsole.Ask<int>("Enter [yellow]Max Guests[/]:");
                var isAvailable = AnsiConsole.Confirm("Is the room available?");

                var room = new Room
                {
                    Type = type,
                    Price = price,
                    ExtraBeds = extraBeds,
                    MaxGuests = maxGuests,
                    IsAvailable = isAvailable
                };

                dbHelper.AddRoom(room);
                AnsiConsole.Markup("[green]Room added successfully![/]\n");
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup($"[red]Error: {ex.Message}[/]\n");
            }
        }

        private void ViewAllRooms()
        {
            try
            {
                AnsiConsole.Clear();
                AnsiConsole.Markup("[bold yellow]View All Room[/]\n");
                var rooms = dbHelper.GetAllRooms();

                if (!rooms.Any())
                {
                    AnsiConsole.Markup("[red]No rooms found![/]\n");
                    return;
                }

                var table = new Table();
                table.AddColumn("[cyan]Room Number[/]");
                table.AddColumn("[cyan]Type[/]");
                table.AddColumn("[cyan]Price[/]");
                table.AddColumn("[cyan]Extra Beds[/]");
                table.AddColumn("[cyan]Max Guests[/]");
                table.AddColumn("[cyan]Available[/]");

                foreach (var room in rooms)
                {
                    table.AddRow(
                        room.RoomNumber.ToString(),
                        room.Type,
                        $"{room.Price:C}",
                        room.ExtraBeds.ToString(),
                        room.MaxGuests.ToString(),
                        room.IsAvailable ? "[green]Yes[/]" : "[red]No[/]");
                }

                AnsiConsole.Write(table);
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup($"[red]Error: {ex.Message}[/]\n");
            }

        }
        private void SearchAvailableRooms()
        {
            try
            {
                AnsiConsole.Clear();
                AnsiConsole.Markup("[bold yellow]Search for Rooms[/]\n");

                // Använd SelectionPrompt för att välja rumstyp
                var roomType = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select [yellow]Room Type[/] (or skip):")
                        .AddChoices(new[] { "Single", "Double", "Suite", "Skip" }));
                roomType = roomType == "Skip" ? null : roomType; // Sätt till null om användaren väljer "Skip"

                var minPriceInput = AnsiConsole.Prompt(
                    new TextPrompt<string>("Enter [yellow]Minimum Price[/] (or press Enter to skip):")
                        .AllowEmpty());
                decimal? minPrice = string.IsNullOrWhiteSpace(minPriceInput) ? null : decimal.Parse(minPriceInput);

                var maxPriceInput = AnsiConsole.Prompt(
                    new TextPrompt<string>("Enter [yellow]Maximum Price[/] (or press Enter to skip):")
                        .AllowEmpty());
                decimal? maxPrice = string.IsNullOrWhiteSpace(maxPriceInput) ? null : decimal.Parse(maxPriceInput);

                var onlyAvailable = AnsiConsole.Confirm("Only show available rooms?");

                var filteredRooms = dbHelper.SearchRooms(
                    type: roomType,
                    minPrice: minPrice,
                    maxPrice: maxPrice,
                    isAvailable: onlyAvailable);

                if (!filteredRooms.Any())
                {
                    AnsiConsole.Markup("[red]No rooms match your filters![/]\n");
                    return;
                }

                var table = new Table();
                table.AddColumn("[cyan]Room Number[/]");
                table.AddColumn("[cyan]Type[/]");
                table.AddColumn("[cyan]Price[/]");
                table.AddColumn("[cyan]Max Guests[/]");
                table.AddColumn("[cyan]Availability[/]");

                foreach (var room in filteredRooms)
                {
                    table.AddRow(
                        room.RoomNumber.ToString(),
                        room.Type,
                        $"{room.Price:C}",
                        room.MaxGuests.ToString(),
                        room.IsAvailable ? "[green]Available[/]" : "[red]Occupied[/]");
                }

                AnsiConsole.Write(table);
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup($"[red]Error: {ex.Message}[/]\n");
            }
        }
        private void DeleteRoom()
        {
            try
            {
                AnsiConsole.Clear();

                var roomNumber = AnsiConsole.Ask<int>("Enter [yellow]Room Number[/] to delete:");

                var confirm = AnsiConsole.Confirm($"Are you sure you want to delete Room [red]{roomNumber}[/]?");
                if (!confirm) return;

                dbHelper.DeleteRoom(roomNumber);
                AnsiConsole.Markup("[green]Room deleted successfully![/]\n");
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup($"[red]Error: {ex.Message}[/]\n");
            }
        }

        private void EditRoom()
        {
            try
            {
                AnsiConsole.Clear();

                var roomNumber = AnsiConsole.Ask<int>("Enter [yellow]Room Number[/] to edit:");

                var room = dbHelper.FindRoom(roomNumber);
                if (room == null)
                {
                    AnsiConsole.Markup("[red]Room not found![/]\n");
                    return;
                }

                var typeInput = AnsiConsole.Prompt(
                    new TextPrompt<string>($"Enter new [yellow]Room Type[/] (current: {room.Type}):")
                        .AllowEmpty());
                room.Type = string.IsNullOrWhiteSpace(typeInput) ? room.Type : typeInput;

                var priceInput = AnsiConsole.Prompt(
                    new TextPrompt<string>($"Enter new [yellow]Room Price[/] (current: {room.Price:C}):")
                        .AllowEmpty());
                room.Price = string.IsNullOrWhiteSpace(priceInput) ? room.Price : decimal.Parse(priceInput);

                var extraBedsInput = AnsiConsole.Prompt(
                    new TextPrompt<string>($"Enter new [yellow]Extra Beds[/] (current: {room.ExtraBeds}):")
                        .AllowEmpty());
                room.ExtraBeds = string.IsNullOrWhiteSpace(extraBedsInput) ? room.ExtraBeds : int.Parse(extraBedsInput);

                var maxGuestsInput = AnsiConsole.Prompt(
                    new TextPrompt<string>($"Enter new [yellow]Max Guests[/] (current: {room.MaxGuests}):")
                        .AllowEmpty());
                room.MaxGuests = string.IsNullOrWhiteSpace(maxGuestsInput) ? room.MaxGuests : int.Parse(maxGuestsInput);

                // Confirm availability
                room.IsAvailable = AnsiConsole.Confirm($"Is the room available? (current: {(room.IsAvailable ? "Yes" : "No")})");

                // Update the room in the database
                dbHelper.UpdateRoom(room);
                AnsiConsole.Markup("[green]Room updated successfully![/]\n");
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup($"[red]Error: {ex.Message}[/]\n");
            }
        }

        private void CheckInRoom()
        {
            Console.Clear();
            Console.WriteLine("===================");
            Console.WriteLine(" Check-In a Room");
            Console.WriteLine("===================");

            try
            {
                int roomNumber = validator.GetValidIntInput("Enter the Room Number to check-in: ");
                var rooms = dbHelper.GetAllRooms();
                var room = rooms.Find(r => r.RoomNumber == roomNumber);

                if (room != null && room.IsAvailable)
                {
                    room.IsAvailable = false; // Mark as not available
                    dbHelper.AddRoom(room);
                    Console.WriteLine("\nRoom checked-in successfully!");
                }
                else
                {
                    Console.WriteLine("\nRoom is either not available or does not exist.");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Failed to check-in room.");
            }
        }
    }
}