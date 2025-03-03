using HotelBooking.Data;
using HotelBooking.Models;

namespace HotelBooking.Repositories;

public interface IRoomRepository : IGenericRepository<Room>
{
   
}

public class RoomRepository : GenericRepository<Room>, IRoomRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public RoomRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
    {
        /*_applicationDbContext = applicationDbContext;*/
    }

    
}