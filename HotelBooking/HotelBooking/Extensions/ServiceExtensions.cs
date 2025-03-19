using HotelBooking.Repositories;
using HotelBooking.Services;

namespace HotelBooking.Extensions;

public static class ServiceExtensions
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<UserContext>();
//Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IRoomTypeService, RoomTypeService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IReservationRoomTypeService, ReservationRoomTypeService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));
        services.AddScoped<IUserContext, UserContext>();

//Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IReservationRoomTypeRepository, ReservationRoomTypeRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    }
}