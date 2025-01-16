using BeachHotel_App.Data;
using BeachHotel_App.Menu.Booking_Menu;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BeachHotel_App.Menu;


namespace BeachHotel_App
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Configure the database context
            var builder = new ConfigurationBuilder()
                          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var config = builder.Build();

            var options = new DbContextOptionsBuilder<HotelDbContext>();
            options.UseSqlServer(config.GetConnectionString("DefaultConnection"));

            using (var dbContext = new HotelDbContext(options.Options))
            {
                var initializer = new DataInitializer();
                initializer.MigrateAndSeed(dbContext); 
            }

          
            using (var dbContext = new HotelDbContext(options.Options))
            {
                var mainMenu = new MainMenu();
                mainMenu.DisplayMenu();


            }
        }
    }
}

