using HotelBooking.Data;
using HotelBooking.Models;

namespace HotelBooking.Repositories;

public interface IAdminRepository : IGenericRepository<Admin>;

public class AdminRepository : GenericRepository<Admin>, IAdminRepository
{
    public AdminRepository(ApplicationDbContext context) : base(context){}
}