using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeachHotel_App.Services
{
    public class ErrorHandler
    {

       
        /// Handles exceptions and displays/logs the error message.
        /// </summary>
        public static void Handle(Exception ex, string userFriendlyMessage = "An error occurred.")
        {
            // Log the actual exception (optional: save to a log file or database)
            Console.WriteLine($"[Error]: {ex.Message}");

            // Display a user-friendly message
            Console.WriteLine(userFriendlyMessage);
        }
    }
}

