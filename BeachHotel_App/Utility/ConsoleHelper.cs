using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeachHotel_App.Utility
{
    public class ConsoleHelper
    {
        public static void PrintHeader(string title)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;

            string border = new string('=', 40);
            Console.WriteLine(border);
            Console.WriteLine($"{"",15}{title}"); 
            Console.WriteLine(border);

            Console.ResetColor();
        }

        public static void PrintMenuOption(string optionNumber, string description)
        {
            Console.WriteLine($"   {optionNumber}. {description}");
        }

        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {message}");
            Console.ResetColor();
        }

        public static void PrintAsciiTitle(string[] asciiLines)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;

           
            int requiredWidth = asciiLines.Max(line => line.Length) + 10; 
            if (Console.WindowWidth < requiredWidth)
            {
                Console.SetWindowSize(Math.Min(requiredWidth, Console.LargestWindowWidth), Console.WindowHeight);
            }

            // Centrera ASCII-text
            foreach (var line in asciiLines)
            {
                int padding = (Console.WindowWidth - line.Length) / 2;
                Console.WriteLine($"{new string(' ', Math.Max(padding, 0))}{line}");
            }

            Console.ResetColor();
            Console.WriteLine("\n");
        }
        public static void PrintSpectreTitle(string title)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
    new FigletText("Beach Hotel")
        .Centered()
        .Color(Spectre.Console.Color.Aqua)); 


        }
    }
}
