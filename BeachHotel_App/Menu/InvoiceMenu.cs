using BeachHotel_App.Data;
using BeachHotel_App.Services;
using BeachHotel_App.Utility;
using BeachHotel_App.Model;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeachHotel_App.Menu
{
    public class InvoiceMenu
    {
        public static void DisplayMenu(HotelDbContext dbContext)
        {
            var service = new PaymentInvoiceService(dbContext);

            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText("Invoice Menu").Centered().Color(Color.Yellow));

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Select an option:[/]")
                        .AddChoices(
                            "Add Payment Invoice",
                            "View All Invoices",
                            "Update Payment Invoice",
                            "Delete Payment Invoice",
                            "Cancel Unpaid Bookings",
                            "Back to Main Menu"));

                if (choice == "Back to Main Menu") return;

                switch (choice)
                {
                    case "Add Payment Invoice":
                        AddInvoice(service);
                        break;
                    case "View All Invoices":
                        ViewAllInvoices(service);
                        break;
                    case "Update Payment Invoice":
                        UpdateInvoice(service);
                        break;
                    case "Delete Payment Invoice":
                        DeleteInvoice(service);
                        break;
                    case "Cancel Unpaid Bookings":
                        service.CancelUnpaidBookings();
                        break;
                }

                AnsiConsole.Markup("\n[green]Press any key to return to the menu...[/]");
                Console.ReadKey();
            }
        }

        private static void AddInvoice(PaymentInvoiceService service)
        {
            var invoice = new PaymentInvoice
            {
                BookingId = PromptInput<int>("Enter [yellow]Booking ID[/]:"),
                Amount = PromptInput<decimal>("Enter [yellow]Amount[/]:"),
                PaymentDate = DateTime.Today,
                PaymentMethod = PromptInput<string>("Enter [yellow]Payment Method[/]:"),
                PaymentStatus = PromptInput<string>("Enter [yellow]Payment Status[/]:"),
                InvoiceDate = DateTime.Today,
                InvoiceNotes = PromptInput<string>("Enter [yellow]Invoice Notes[/] (optional):", "")
            };

            service.AddInvoice(invoice);
        }

        private static void ViewAllInvoices(PaymentInvoiceService service)
        {
            AnsiConsole.Clear();
            AnsiConsole.Markup("[bold yellow]View All Invoices[/]\n");

           
            var invoices = service.GetAllInvoices();

            // Check if there are invoices to display
            if (!invoices.Any())
            {
                AnsiConsole.Markup("[red]No invoices found.[/]\n");
                return;
            }

            // Create a table with all columns, including Invoice Date
            var table = new Table()
                .Centered()
                .AddColumn("[cyan]Invoice ID[/]")
                .AddColumn("[cyan]Booking ID[/]")
                .AddColumn("[cyan]Amount[/]")
                .AddColumn("[cyan]Payment Date[/]")
                .AddColumn("[cyan]Payment Method[/]")
                .AddColumn("[cyan]Payment Status[/]")
                .AddColumn("[cyan]Invoice Date[/]")
                .AddColumn("[cyan]Invoice Notes[/]");

            // Add rows to the table
            foreach (var invoice in invoices)
            {
                table.AddRow(
                    invoice.PaymentInvoiceId.ToString(),
                    invoice.BookingId.ToString(),
                    $"{invoice.Amount:C}", 
                    invoice.PaymentDate?.ToShortDateString() ?? "[red]Not Paid[/]", 
                    invoice.PaymentMethod ?? "[yellow]N/A[/]", 
                    invoice.PaymentStatus ?? "[yellow]N/A[/]", 
                    invoice.InvoiceDate.ToShortDateString() ?? "[yellow]N/A[/]", 
                    invoice.InvoiceNotes?.Replace("\n", " ") ?? "[yellow]N/A[/]" 
                );
            }

            // Display the table
            AnsiConsole.Write(table);
        }



        private static void UpdateInvoice(PaymentInvoiceService service)
        {
            var id = PromptInput<int>("Enter [yellow]Invoice ID[/] to update:");
            var newAmount = PromptInput<decimal>("Enter [yellow]New Amount[/]:");
            var newStatus = PromptInput<string>("Enter [yellow]New Status[/]:");
            var newMethod = PromptInput<string>("Enter [yellow]New Method[/]:");
            var newNotes = PromptInput<string>("Enter [yellow]New Notes[/] (optional):", "");

            service.UpdateInvoice(id, newAmount, newStatus, newMethod, newNotes);
        }

        private static void DeleteInvoice(PaymentInvoiceService service)
        {
            var id = PromptInput<int>("Enter [yellow]Invoice ID[/] to delete:");
            service.DeleteInvoice(id);
        }

        private static T PromptInput<T>(string message, T defaultValue = default)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)AnsiConsole.Ask<string>(message, (string)(object)defaultValue);
            }

            return AnsiConsole.Ask<T>(message);
        }
    }
}

