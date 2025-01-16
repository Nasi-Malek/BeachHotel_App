using BeachHotel_App.Model;
using BeachHotel_App.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeachHotel_App.Data
{

    public class HotelDbContext : DbContext
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<BookingService> BookingServices { get; set; }
        public DbSet<PaymentInvoice> PaymentInvoices { get; set; }

        public HotelDbContext() { }

        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");

                optionsBuilder.UseSqlServer(connectionString);

            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Room>()
        .HasKey(r => r.RoomNumber);

            // Room Configuration
            modelBuilder.Entity<Room>()
                .Property(r => r.Type)
                .IsRequired();

            modelBuilder.Entity<Room>()
                .Property(r => r.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Room>()
                .Property(r => r.ExtraBeds)
                .IsRequired();

            modelBuilder.Entity<Room>()
                .Property(r => r.Size)
                .IsRequired();

            modelBuilder.Entity<Room>()
                .Property(r => r.MaxGuests)
                .IsRequired();



            // Service Configuration
            modelBuilder.Entity<Service>()
                .HasKey(s => s.ServiceId);

            modelBuilder.Entity<Service>()
                .Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");


            // PaymentInvoice Configuration
            modelBuilder.Entity<PaymentInvoice>()
                .HasKey(p => p.PaymentInvoiceId);

            modelBuilder.Entity<PaymentInvoice>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PaymentInvoice>()
                .Property(p => p.PaymentMethod)
                .HasMaxLength(50);

            modelBuilder.Entity<PaymentInvoice>()
                .Property(p => p.PaymentStatus)
                .HasMaxLength(20);

            modelBuilder.Entity<PaymentInvoice>()
                .Property(p => p.InvoiceNotes)
                .HasMaxLength(500);

            // Define one-to-many relationship
            modelBuilder.Entity<PaymentInvoice>()
                .HasOne(pi => pi.Booking)
                .WithMany(b => b.PaymentInvoices)
                .HasForeignKey(pi => pi.BookingId);


            // BookingService Many-to-Many Relationship

            modelBuilder.Entity<BookingService>()
        .HasKey(bs => bs.BookingServiceId); 

            modelBuilder.Entity<BookingService>()
                .HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingServices)
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookingService>()
                .HasOne(bs => bs.Service)
                .WithMany(s => s.BookingServices)
                .HasForeignKey(bs => bs.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure BookingId as an auto-generated identity column
            modelBuilder.Entity<Booking>()
                .Property(b => b.BookingId)
                    .ValueGeneratedOnAdd();            
        }
    }
}