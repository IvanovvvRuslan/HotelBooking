using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Configurations;

public class ReservationRoomTypeConfiguration : IEntityTypeConfiguration<ReservationRoomType>
{
    public void Configure(EntityTypeBuilder<ReservationRoomType> builder)
    {
        builder.HasKey(r => new { r.ReservationId, r.RoomTypeId });

        builder.HasOne(r => r.Reservation)
            .WithMany(r => r.RoomTypes)
            .HasForeignKey(r => r.ReservationId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(r => r.RoomType)
            .WithMany(r => r.Reservations)
            .HasForeignKey(r => r.RoomTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}