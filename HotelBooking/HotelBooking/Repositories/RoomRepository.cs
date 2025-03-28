using HotelBooking.Data;
using HotelBooking.Models;

namespace HotelBooking.Repositories;

public interface IRoomRepository : IGenericRepository<Room>;


public class RoomRepository : GenericRepository<Room>, IRoomRepository
{
    public RoomRepository(ApplicationDbContext context) : base(context) {}
}