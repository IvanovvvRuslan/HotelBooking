using HotelBooking.Data;
using HotelBooking.Models;

namespace HotelBooking.Repositories;

public interface IUserRepository : IGenericRepository<User>;

public class UserRepository: GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) {}
}