using BeachHotel_App.Data;
using BeachHotel_App.Model;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BeachHotel_App.Menu
{
    public class GuestMenu
    {
        public void DisplayGuestMenu()
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText("Guest Menu").Centered().Color(Color.Yellow));

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Select an option:[/]")
                        .AddChoices("Add a New Guest", "View All Guests", "Edit Guest Information", "Delete a Guest", "Back to Main Menu"));

                switch (choice)
                {
                    case "Add a New Guest":
                        AddGuest();
                        break;
                    case "View All Guests":
                        ViewAllGuests();
                        break;
                    case "Edit Guest Information":
                        EditGuest();
                        break;
                    case "Delete a Guest":
                        DeleteGuest();
                        break;
                    case "Back to Main Menu":
                        return;
                }

                AnsiConsole.Markup("\n[green]Press any key to return to the menu...[/]");
                Console.ReadKey();
            }
        }

        private void AddGuest()
        {
            try
            {
                AnsiConsole.Clear();
                AnsiConsole.Markup("[bold yellow]Add New Guest[/]\n");

                // Get Guest Name
                var name = AnsiConsole.Ask<string>("Enter [yellow]Guest Name[/]:");

                // Get and validate Contact Number
                string contactNumber;
                while (true)
                {
                    contactNumber = AnsiConsole.Ask<string>("Enter [yellow]Contact Number[/] (must be numeric):");
                    if (long.TryParse(contactNumber, out _)) break;
                    AnsiConsole.Markup("[red]Invalid contact number! Please enter a numeric value.[/]\n");
                }

                // Get and validate Email with the option to skip
                string email = AnsiConsole.Prompt(
                    new TextPrompt<string>("Enter [yellow]Email[/] (press Enter to skip):")
                        .AllowEmpty()
                        .Validate(input =>
                        {
                            if (string.IsNullOrEmpty(input) || IsValidEmail(input))
                                return ValidationResult.Success();
                            return ValidationResult.Error("[red]Invalid email format! Please enter a valid email or leave it blank to skip.[/]");
                        }));

                // Save the guest to the database
                using (var context = new HotelDbContext())
                {
                    var guest = new Guest
                    {
                        Name = name,
                        ContactNumber = contactNumber,
                        Email = string.IsNullOrEmpty(email) ? null : email 

                    };

                    context.Guests.Add(guest);
                    context.SaveChanges();
                    AnsiConsole.Markup("[green]Guest added successfully![/]\n");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup($"[red]Error: {ex.Message}[/]\n");
            }
        }

        // Helper method to validate email format
        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }


        private void ViewAllGuests()
        {
            try
            {
                AnsiConsole.Clear();
                AnsiConsole.Markup("[bold yellow]View All Guests[/]\n");

                using (var context = new HotelDbContext())
                {
                    var guests = context.Guests.ToList();

                    if (!guests.Any())
                    {
                        AnsiConsole.Markup("[red]No guests found![/]\n");
                        return;
                    }

                    var table = new Table();
                    table.AddColumn("[cyan]Guest ID[/]");
                    table.AddColumn("[cyan]Name[/]");
                    table.AddColumn("[cyan]Contact Number[/]");
                    table.AddColumn("[cyan]Email[/]");

                    foreach (var guest in guests)
                    {
                        table.AddRow(
                            guest.GuestId.ToString(),
                            guest.Name ?? "N/A", 
                            guest.ContactNumber ?? "N/A", 
                            guest.Email ?? "N/A" 
                        );
                    }

                    AnsiConsole.Write(table);
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup($"[red]Error: {ex.Message}[/]\n");
            }
        }


        private void EditGuest()
        {
            try
            {
                AnsiConsole.Clear();
                AnsiConsole.Markup("[bold yellow]Edit Guest Info[/]\n");

                var guestId = AnsiConsole.Ask<int>("Enter [yellow]Guest ID[/] to edit:");

                using (var context = new HotelDbContext())
                {
                    var guest = context.Guests.Find(guestId);

                    if (guest == null)
                    {
                        AnsiConsole.Markup("[red]Guest not found![/]\n");
                        return;
                    }

                    guest.Name = AnsiConsole.Ask<string>(
                        $"Enter new [yellow]Name[/] (current: {guest.Name}):", guest.Name);
                    guest.ContactNumber = AnsiConsole.Ask<string>(
                        $"Enter new [yellow]Contact Number[/] (current: {guest.ContactNumber}):", guest.ContactNumber);
                    guest.Email = AnsiConsole.Ask<string>(
                        $"Enter new [yellow]Email[/] (current: {guest.Email}):", guest.Email);

                    context.SaveChanges();
                    AnsiConsole.Markup("[green]Guest updated successfully![/]\n");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup($"[red]Error: {ex.Message}[/]\n");
            }
        }

        private void DeleteGuest()
        {
            try
            {
                AnsiConsole.Clear();
                AnsiConsole.Markup("[bold red]Delete a Guest[/]\n");

                var guestId = AnsiConsole.Ask<int>("Enter [yellow]Guest ID[/] to delete:");

                using (var context = new HotelDbContext())
                {
                    var guest = context.Guests
                        .Include(g => g.Bookings) 
                        .FirstOrDefault(g => g.GuestId == guestId);

                    if (guest == null)
                    {
                        AnsiConsole.Markup("[red]Guest not found![/]\n");
                        return;
                    }

                    if (guest.Bookings != null && guest.Bookings.Any())
                    {
                        AnsiConsole.Markup(
                            $"[red]Cannot delete Guest '{guest.Name}' because there are active bookings![/]\n");
                        return;
                    }

                    if (AnsiConsole.Confirm($"Are you sure you want to delete Guest [red]{guest.Name}[/]?") == false)
                    {
                        AnsiConsole.Markup("[yellow]Operation cancelled.[/]\n");
                        return;
                    }

                    context.Guests.Remove(guest);
                    context.SaveChanges();
                    AnsiConsole.Markup($"[green]Guest '{guest.Name}' deleted successfully![/]\n");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup($"[red]Error: {ex.Message}[/]\n");
            }
        }
    }
}
