using HotelBooking.Data;
using HotelBooking.DTO.ResponseDto;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repositories;

public interface IRoomTypeRepository : IGenericRepository<RoomType>;

public class RoomTypeRepository : GenericRepository<RoomType>, IRoomTypeRepository
{
    public RoomTypeRepository(ApplicationDbContext context) : base(context) {}
}