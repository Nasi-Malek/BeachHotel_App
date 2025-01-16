using BeachHotel_App.Data;
using BeachHotel_App.Model;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeachHotel_App.Menu
{
    public class ServiceMenu
    {
        public void DisplayServiceMenu()
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText("Service Menu").Centered().Color(Color.Cyan1));

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Select an option:[/]")
                        .AddChoices("Add New Service", "View All Services", "Update Service", "Delete Service", "Back to Main Menu"));

                try
                {
                    switch (choice)
                    {
                        case "Add New Service":
                            AddService();
                            break;
                        case "View All Services":
                            ViewAllServices();
                            break;
                        case "Update Service":
                            UpdateService();
                            break;
                        case "Delete Service":
                            DeleteService();
                            break;
                        case "Back to Main Menu":
                            return; 
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

        private void AddService()
        {
            AnsiConsole.Clear();
            AnsiConsole.Markup("[bold yellow]Add New Service[/]\n");

            var name = PromptInput<string>("Enter Service Name:");
            var price = PromptInput<decimal>("Enter Service Price:");
            var description = PromptInput<string>("Enter Service Description:");

            using (var context = new HotelDbContext())
            {
                var service = new Service { Name = name, Price = price, Description = description };
                context.Services.Add(service);
                context.SaveChanges();
                AnsiConsole.Markup("[green]Service added successfully![/]\n");
            }
        }

        private void ViewAllServices()
        {
            AnsiConsole.Clear();
            AnsiConsole.Markup("[bold yellow]View All Services[/]\n");

            using (var context = new HotelDbContext())
            {
                var services = context.Services.ToList();

                if (!services.Any())
                {
                    AnsiConsole.Markup("[red]No services found.[/]\n");
                    return;
                }

                var table = new Table();
                table.AddColumn("[cyan]ID[/]");
                table.AddColumn("[cyan]Name[/]");
                table.AddColumn("[cyan]Price[/]");
                table.AddColumn("[cyan]Description[/]");

                foreach (var service in services)
                {
                    table.AddRow(service.ServiceId.ToString(), service.Name, $"{service.Price:C}", service.Description);
                }

                AnsiConsole.Write(table);
            }
        }

        private void UpdateService()
        {
            AnsiConsole.Clear();
            AnsiConsole.Markup("[bold yellow]Update Service[/]\n");

            var serviceId = PromptInput<int>("Enter Service ID to update:");

            using (var context = new HotelDbContext())
            {
                var service = context.Services.Find(serviceId);

                if (service == null)
                {
                    AnsiConsole.Markup("[red]Service not found.[/]\n");
                    return;
                }

                var newName = PromptInput<string>($"Enter new Service Name (current: {service.Name}):", service.Name);
                var newPrice = PromptInput<decimal>($"Enter new Service Price (current: {service.Price:C}):", service.Price);
                var newDescription = PromptInput<string>($"Enter new Description (current: {service.Description}):", service.Description);

                service.Name = newName;
                service.Price = newPrice;
                service.Description = newDescription;

                context.SaveChanges();
                AnsiConsole.Markup("[green]Service updated successfully![/]\n");
            }
        }

        private void DeleteService()
        {
            AnsiConsole.Clear();
            AnsiConsole.Markup("[bold red]Delete Service[/]\n");

            var serviceId = PromptInput<int>("Enter Service ID to delete:");

            using (var context = new HotelDbContext())
            {
                var service = context.Services.Find(serviceId);

                if (service == null)
                {
                    AnsiConsole.Markup("[red]Service not found.[/]\n");
                    return;
                }

                context.Services.Remove(service);
                context.SaveChanges();
                AnsiConsole.Markup("[green]Service deleted successfully![/]\n");
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
