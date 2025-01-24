using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options) {}
    
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<ReservationRoomType> ReservationRoomTypes { get; set; }
    public DbSet<RoomType> RoomTypes { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Admin>()
            .HasOne(a => a.User)
            .WithOne(u => u.Admin)
            .HasForeignKey<Admin>(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Client>()
            .HasOne(a => a.User)
            .WithOne(c => c.Client)
            .HasForeignKey<Client>(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Client>()
            .HasMany(c => c.Reservations)
            .WithOne(r => r.Client)
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<ReservationRoomType>()
            .HasKey(r => new { r.ReservationId, r.RoomTypeId });

        modelBuilder.Entity<ReservationRoomType>()
            .HasOne(r => r.Reservation)
            .WithMany(r => r.RoomTypes)
            .HasForeignKey(r => r.ReservationId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<ReservationRoomType>()
            .HasOne(r => r.RoomType)
            .WithMany(r => r.Reservations)
            .HasForeignKey(r => r.RoomTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .ToTable("Users");
    }
}