using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using BeachHotel_App.Data;
using BeachHotel_App.Services;
using BeachHotel_App.Menu.Booking_Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeachHotel_App.Utility;


namespace BeachHotel_App.Menu.Booking_Menu
{
    public class MainMenu
    {

        private static readonly string[] TitleAscii = new string[]
        {
        "  ____       _    __  __       _      ____            _       ",
        " |  _ \\ __ _| |_ |  \\/  | __ _| |__  |  _ \\ __ _  ___| | ____ ",
        " | |_) / _` | __|| |\\/| |/ _` | '_ \\ | |_) / _` |/ __| |/ /\\ \\",
        " |  __/ (_| | |_ | |  | | (_| | | | ||  __/ (_| | (__|   <  > |",
        " |_|   \\__,_|\\__||_|  |_|\\__,_|_| |_| \\_|   \\__,_|\\___|_|\\_\\_/"
        };


        public MainMenu()
        {

            var optionsBuilder = new DbContextOptionsBuilder<HotelDbContext>();
            optionsBuilder.UseSqlServer("YourConnectionStringHere"); 
        }

        public void DisplayMenu()
        {
            // Visa titel
            ConsoleHelper.PrintSpectreTitle("Beach Hotel");

            while (true)
            {

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Select an option:[/]")
                        .AddChoices(new[] {
                        "1. Rooms",
                        "2. Guests",
                        "3. Bookings",
                        "4. Invoices",
                        "5. View Booking History",
                        "6. Service Management",
                        "7. Exit"
                        }));

                switch (choice)
                {
                    case "1. Rooms":
                        new RoomMenu().DisplayRoomMenu();
                        break;
                    case "2. Guests":
                        new GuestMenu().DisplayGuestMenu();
                        break;
                    case "3. Bookings":
                        new BookingMenu().DisplayBookingMenu();

                        break;
                    case "4. Invoices":
                        using (var dbContext = new HotelDbContext())
                        {
                            InvoiceMenu.DisplayMenu(dbContext);
                        }
                        break;
                    case "5. View Booking History":
                        using (var dbContext = new HotelDbContext())
                        {
                            var bookingHistoryService = new BookingHistoryService(dbContext);
                            bookingHistoryService.ViewBookingHistory();
                        }
                        break;
                    case "6. Service Management":
                        DisplayServiceManagementMenu();
                        break;
                    case "7. Exit":
                        AnsiConsole.Markup("[green]Exiting... Goodbye![/]");
                        return;



                }
            }
        }
        // Service Management Menu

        private void DisplayServiceManagementMenu()
        {
            var serviceChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Service Management Options:[/]")
                    .AddChoices(new[] {
                        "1. Manage Services (CRUD)",
                        "2. Manage Booking Services",
                        "3. Back to Main Menu"
                    }));

            switch (serviceChoice)
            {
                case "1. Manage Services (CRUD)":
                    new ServiceMenu().DisplayServiceMenu();
                    break;
                case "2. Manage Booking Services":
                    new BookingServiceMenu().DisplayBookingServiceMenu();
                    break;
                case "3. Back to Main Menu":
                    return;
            }

        }
    }

}


