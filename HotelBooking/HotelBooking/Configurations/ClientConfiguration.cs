using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasOne(a => a.User)
            .WithOne(c => c.Client)
            .HasForeignKey<Client>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Reservations)
            .WithOne(r => r.Client)
            .HasForeignKey(r => r.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}