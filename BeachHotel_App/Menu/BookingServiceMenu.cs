using Microsoft.EntityFrameworkCore;
using BeachHotel_App.Data;
using BeachHotel_App.Model;
using BeachHotel_App.Services;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeachHotel_App.Menu
{
    public class BookingServiceMenu
    {
        public void DisplayBookingServiceMenu()
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText("Booking Service Menu").Centered().Color(Color.Green));

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Select an option:[/]")
                        .AddChoices("Add Booking Service", "View Booking Services", "Edit Booking Service", "Delete Booking Service", "Back to Main Menu"));

                if (choice == "Back to Main Menu") return;

                try
                {
                    switch (choice)
                    {
                        case "Add Booking Service":
                            AddBookingService();
                            break;
                        case "View Booking Services":
                            ViewBookingServices();
                            break;
                        case "Edit Booking Service":
                            EditBookingService();
                            break;
                        case "Delete Booking Service":
                            DeleteBookingService();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }

                AnsiConsole.Markup("\n[green]Press any key to return to the menu...[/]");
                Console.ReadKey();
            }
        }

        private static void HandleError(Exception ex)
        {
            AnsiConsole.Markup($"[red]Error: {ex.Message}[/]\n");
        }

        private void AddBookingService()
        {
            AnsiConsole.Clear();
            AnsiConsole.Markup("[bold yellow]Add Booking Service[/]\n");

            try
            {
                var bookingId = PromptInput<int>("Enter Booking ID:");
                var serviceId = PromptInput<int>("Enter Service ID:");
                var quantity = PromptInput<int>("Enter Quantity (must be > 0):");
                if (quantity <= 0)
                {
                    AnsiConsole.Markup("[red]Quantity must be greater than 0.[/]\n");
                    return;
                }

                var name = PromptInput<string>("Enter Booking Service Name:");

                using (var context = new HotelDbContext())
                {
                    if (!context.Bookings.Any(b => b.BookingId == bookingId))
                    {
                        AnsiConsole.Markup("[red]Booking not found.[/]\n");
                        return;
                    }

                    if (!context.Services.Any(s => s.ServiceId == serviceId))
                    {
                        AnsiConsole.Markup("[red]Service not found.[/]\n");
                        return;
                    }

                    var bookingService = new BookingService
                    {
                        BookingId = bookingId,
                        ServiceId = serviceId,
                        Quantity = quantity,
                        ServiceName = name
                    };

                    context.BookingServices.Add(bookingService);
                    context.SaveChanges();
                    AnsiConsole.Markup("[green]Booking Service added successfully![/]\n");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private void ViewBookingServices()
        {
            AnsiConsole.Clear();
            AnsiConsole.Markup("[bold yellow]View Booking Services[/]\n");

            using (var context = new HotelDbContext())
            {
                var bookingServices = context.BookingServices
                    .Include(bs => bs.Service)
                    .Include(bs => bs.Booking)
                    .ToList();

                if (!bookingServices.Any())
                {
                    AnsiConsole.Markup("[red]No booking services found.[/]\n");
                    return;
                }

                var table = new Table();
                table.AddColumn("[cyan]ID[/]");
                table.AddColumn("[cyan]Booking ID[/]");
                table.AddColumn("[cyan]Service ID[/]");
                table.AddColumn("[cyan]Service Name[/]");
                table.AddColumn("[cyan]Quantity[/]");

                foreach (var bs in bookingServices)
                {
                    table.AddRow(
                        bs.BookingServiceId.ToString(),
                        bs.BookingId.ToString(),
                        bs.ServiceId.ToString(),
                        bs.ServiceName,
                        bs.Quantity.ToString());
                }

                AnsiConsole.Write(table);
            }
        }

        private void DeleteBookingService()
        {
            AnsiConsole.Clear();
            AnsiConsole.Markup("[bold red]Delete Booking Service[/]\n");

            try
            {
                var bookingServiceId = PromptInput<int>("Enter Booking Service ID to delete:");

                using (var context = new HotelDbContext())
                {
                    var bookingService = context.BookingServices.Find(bookingServiceId);

                    if (bookingService == null)
                    {
                        AnsiConsole.Markup("[red]Booking Service not found.[/]\n");
                        return;
                    }

                    context.BookingServices.Remove(bookingService);
                    context.SaveChanges();
                    AnsiConsole.Markup("[green]Booking Service deleted successfully![/]\n");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private void EditBookingService()
        {
            AnsiConsole.Clear();
            AnsiConsole.Markup("[bold yellow]Edit Booking Service[/]\n");

            try
            {
                var bookingServiceId = PromptInput<int>("Enter Booking Service ID to edit:");

                using (var context = new HotelDbContext())
                {
                    var bookingService = context.BookingServices.Find(bookingServiceId);

                    if (bookingService == null)
                    {
                        AnsiConsole.Markup("[red]Booking Service not found.[/]\n");
                        return;
                    }

                    AnsiConsole.Markup("[bold cyan]Current Booking Service Details:[/]\n");
                    AnsiConsole.Markup($"[cyan]Booking Service ID:[/] [yellow]{bookingService.BookingServiceId}[/]\n");
                    AnsiConsole.Markup($"[cyan]Booking ID:[/] [yellow]{bookingService.BookingId}[/]\n");
                    AnsiConsole.Markup($"[cyan]Service ID:[/] [yellow]{bookingService.ServiceId}[/]\n");
                    AnsiConsole.Markup($"[cyan]Service Name:[/] [yellow]{bookingService.ServiceName}[/]\n");
                    AnsiConsole.Markup($"[cyan]Quantity:[/] [yellow]{bookingService.Quantity}[/]\n");

                    AnsiConsole.Markup("\n[bold]Enter new details or press Enter to keep the current values.[/]\n");

                    var newServiceId = PromptInput<int>($"New [yellow]Service ID[/] (current: {bookingService.ServiceId}):", bookingService.ServiceId);
                    var newServiceName = PromptInput<string>($"New [yellow]Service Name[/] (current: {bookingService.ServiceName}):", bookingService.ServiceName);
                    var newQuantity = PromptInput<int>($"New [yellow]Quantity[/] (current: {bookingService.Quantity}):", bookingService.Quantity);

                    bookingService.ServiceId = newServiceId;
                    bookingService.ServiceName = newServiceName;
                    bookingService.Quantity = newQuantity;

                    context.SaveChanges();
                    AnsiConsole.Markup("[green]Booking Service updated successfully![/]\n");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private static T PromptInput<T>(string message, T defaultValue = default)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<T>(message)
                    .PromptStyle("yellow")
                    .DefaultValue(defaultValue));
        }
    }
}
