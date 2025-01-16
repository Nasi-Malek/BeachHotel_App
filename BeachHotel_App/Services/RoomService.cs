using BeachHotel_App.Data;
using BeachHotel_App.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeachHotel_App.Services
{
    public class RoomService
    {
        
        private readonly DbContextOptions<HotelDbContext> _options;

        // Constructor to inject DbContext options
        public RoomService(DbContextOptions<HotelDbContext> options)
        {
            _options = options;
        }

        public void AddRoom(Room room)
        {

            using (var context = new HotelDbContext(_options))
            {
                try
                {
                    Console.WriteLine("Attempting to add room:");
                    Console.WriteLine($"Type: {room.Type}, Price: {room.Price}, ExtraBeds: {room.ExtraBeds}, Size: {room.Size}, MaxGuests: {room.MaxGuests}, IsAvailable: {room.IsAvailable}");

                    context.Rooms.Add(room);
                    int result = context.SaveChanges();
                    Console.WriteLine($"SaveChanges Result: {result}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        public List<Room> GetAllRooms()
        {
            using (var context = new HotelDbContext(_options))
            {
                try
                {
                    return context.Rooms.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: Failed to retrieve rooms. {ex.Message}");
                    return new List<Room>();
                }
            }
        }

        public void DeleteRoom(int roomNumber)
        {
            using (var context = new HotelDbContext(_options))
            {
                try
                {
                    var room = context.Rooms.Find(roomNumber);
                    if (room != null)
                    {
                        context.Rooms.Remove(room);
                        context.SaveChanges();
                        Console.WriteLine($"Room {roomNumber} deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Room {roomNumber} not found.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: Failed to delete room. {ex.Message}");
                }
            }
        }

        public void UpdateRoom(Room room)
        {
            using (var context = new HotelDbContext(_options))
            {
                try
                {
                    context.Rooms.Update(room);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: Failed to update room. {ex.Message}");
                    throw; 

                }
            }
        }

        public Room FindRoom(int roomNumber)
        {
            using (var context = new HotelDbContext(_options))
            {
                try
                {
                    return context.Rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: Failed to find room. {ex.Message}");
                    return null;
                }
            }
        }
        public List<Room> SearchRooms(string type = null, decimal? minPrice = null, decimal? maxPrice = null, bool? isAvailable = null)
        {
            using (var context = new HotelDbContext(_options))
            {
                try
                {
                    var query = context.Rooms.AsQueryable();

                    if (!string.IsNullOrEmpty(type))
                        query = query.Where(r => r.Type == type);

                    if (minPrice.HasValue)
                        query = query.Where(r => r.Price >= minPrice.Value);

                    if (maxPrice.HasValue)
                        query = query.Where(r => r.Price <= maxPrice.Value);

                    if (isAvailable.HasValue)
                        query = query.Where(r => r.IsAvailable == isAvailable.Value);

                    return query.ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: Failed to search rooms. {ex.Message}");
                    return new List<Room>();
                }
            }

        }
    }
}
