using System.Security.Claims;
using HotelBooking.DTO.RequestDto;
using HotelBooking.DTO.ResponseDto;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using HotelBooking.Repositories;
using Mapster;

namespace HotelBooking.Services;

public interface IReservationService : IGenericService<Reservation, ReservationForAdminDto>
{
    Task<IEnumerable<ReservationForClientDto>> GetAllCurrentReservationsAsync(ClaimsPrincipal user);
    Task<ReservationForClientDto> GetCurrentReservationByIdAsync(int id, ClaimsPrincipal user);
    Task CreateReservationAsync(ReservationForAdminDto reservation);
    Task CreateCurrentReservationAsync (ClaimsPrincipal user, ReservationForClientCreateDto reservation);
    Task UpdateReservationAsync(int id, ReservationForAdminDto reservationForAdminDto);
    Task UpdateCurrentReservationAsync (int id, ReservationForClientUpdateDto reservationForClientUpdateDto, ClaimsPrincipal user);
    Task DeleteCurrentReservationAsync(int id, ClaimsPrincipal user);
}

public class ReservationService : GenericService<Reservation, ReservationForAdminDto>, IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IReservationRoomTypeRepository _reservationRoomTypeRepository;
    private readonly IReservationRoomTypeService _reservationRoomTypeService;
    private readonly UserContext _userContext;
    
    public ReservationService(IReservationRepository reservationRepository, IClientRepository clientRepository, 
        IReservationRoomTypeRepository reservationRoomTypeRepository, IReservationRoomTypeService reservationRoomTypeService,
        UserContext userContext) : base (reservationRepository)
    {
        _reservationRepository = reservationRepository;
        _clientRepository = clientRepository;
        _reservationRoomTypeRepository = reservationRoomTypeRepository;
        _reservationRoomTypeService = reservationRoomTypeService;
        _userContext = userContext;
    }

    public async Task<IEnumerable<ReservationForClientDto>> GetAllCurrentReservationsAsync(ClaimsPrincipal user)
    {
        var userId = int.Parse(_userContext.UserId);
        
        var client = await _clientRepository.GetByUserIdAsync(userId);

        if (client == null)
            throw new NotFoundException("Client not found");
        
        var reservations = await _reservationRepository.GetAllByClientAsync(client.Id);
        
        var reservationsDto = reservations.Adapt<IEnumerable<ReservationForClientDto>>();
        
        return reservationsDto;
    }

    public async Task<ReservationForClientDto> GetCurrentReservationByIdAsync(int id, ClaimsPrincipal user)
    {
        var userId = int.Parse(_userContext.UserId);
        
        var client = await _clientRepository.GetByUserIdAsync(userId);

        if (client == null)
            throw new NotFoundException("Client not found");
        
        var reservation = await _reservationRepository.GetByIdClientAsync(id, client.Id);
        
        if (reservation == null)
            throw new NotFoundException("Reservation not found");
        
        var reservationDto = reservation.Adapt<ReservationForClientDto>();
        
        return reservationDto;
    }

    public async Task CreateReservationAsync(ReservationForAdminDto reservation)
    {
        var newReservation = new Reservation
        {
            ClientId = reservation.ClientId,
            CheckInDate = reservation.CheckInDate,
            CheckOutDate = reservation.CheckOutDate,
            Status = reservation.Status,
            TotalPrice = reservation.TotalPrice,
            GuestCount = reservation.GuestCount,
            Description = reservation.Description
        };
        
        await _reservationRepository.CreateAsync(newReservation);
        await _reservationRepository.SaveChangesAsync();
        
        await _reservationRoomTypeService.AddRoomTypesAsync(newReservation.Id, reservation.RoomTypes);
    }

    public async Task CreateCurrentReservationAsync(ClaimsPrincipal user, ReservationForClientCreateDto reservation)
    {
        var userId = int.Parse(_userContext.UserId);
        
        var client = await _clientRepository.GetByUserIdAsync(userId);
        
        if (client == null)
            throw new NotFoundException("Client not found");
        
        var newReservation = new Reservation
        {
            CheckInDate = reservation.CheckInDate,
            CheckOutDate = reservation.CheckOutDate,
            Status = "Created",
            TotalPrice = 0,
            GuestCount = reservation.GuestCount,
            Description = reservation.Description
        };

        newReservation.ClientId = client.Id;
        
        await _reservationRepository.CreateAsync(newReservation);
        await _reservationRepository.SaveChangesAsync();
        
        await _reservationRoomTypeService.AddRoomTypesAsync(newReservation.Id, reservation.RoomTypes);
    }

    public async Task UpdateReservationAsync(int id, ReservationForAdminDto reservationForAdminDto)
    {
        var oldReservation = await _reservationRepository.GetByIdTrackedAsync(id);

        if (oldReservation == null)
            throw new NotFoundException("Reservation not found");
        
        oldReservation.ClientId = reservationForAdminDto.ClientId;
        oldReservation.CheckInDate = reservationForAdminDto.CheckInDate;
        oldReservation.CheckOutDate = reservationForAdminDto.CheckOutDate;
        oldReservation.Status = reservationForAdminDto.Status;
        oldReservation.TotalPrice = reservationForAdminDto.TotalPrice;
        oldReservation.GuestCount = reservationForAdminDto.GuestCount;
        oldReservation.Description = reservationForAdminDto.Description;
        
        await _reservationRepository.SaveChangesAsync();
        
        await _reservationRoomTypeService.UpdateRoomTypeAsync(oldReservation.Id, reservationForAdminDto.RoomTypes);
    }

    public async Task UpdateCurrentReservationAsync(int id, ReservationForClientUpdateDto reservationForClientUpdateDto, ClaimsPrincipal user)
    {
        var userId = int.Parse(_userContext.UserId);
        
        var client = await _clientRepository.GetByUserIdAsync(userId);

        if (client == null)
            throw new NotFoundException("Client not found");
        
        var oldReservation = await _reservationRepository.GetByIdClientCurrentAsync(id, client.Id);

        if (oldReservation == null)
            throw new NotFoundException("Reservation not found");
        
        oldReservation.CheckInDate = reservationForClientUpdateDto.CheckInDate;
        oldReservation.CheckOutDate = reservationForClientUpdateDto.CheckOutDate;
        oldReservation.GuestCount = reservationForClientUpdateDto.GuestCount;
        oldReservation.Description = reservationForClientUpdateDto.Description;
        
        await _reservationRepository.SaveChangesAsync();
    }

    public async Task DeleteCurrentReservationAsync(int id, ClaimsPrincipal user)
    {
        var userId = int.Parse(_userContext.UserId);
        
        var client = await _clientRepository.GetByUserIdAsync(userId);

        if (client == null)
            throw new NotFoundException("Client not found");
        
        var reservation = await _reservationRepository.GetByIdClientCurrentAsync(id, client.Id);

        if (reservation == null)
            throw new NotFoundException("Reservation not found");
        
        await _reservationRepository.Delete(reservation);
        await _reservationRepository.SaveChangesAsync();
    }
}