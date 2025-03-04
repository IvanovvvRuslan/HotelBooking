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
    Task<IEnumerable<ReservationForAdminDto>> GetAllWithRoomTypesAsync();
    Task<ReservationForAdminDto> GetByIdWithRoomTypesAsync(int id);
    Task<IEnumerable<ReservationForClientDto>> GetAllCurrentAsync(ClaimsPrincipal user);
    Task<ReservationForClientDto> GetCurrentByIdAsync(int id, ClaimsPrincipal user);
    Task CreateAsync(ReservationForAdminDto reservation);
    Task CreateCurrentAsync (ClaimsPrincipal user, ReservationForClientCreateDto reservation);
    Task UpdateAsync(int id, ReservationForAdminDto reservationForAdminDto);
    Task UpdateCurrentAsync (int id, ReservationForClientUpdateDto reservationForClientUpdateDto, ClaimsPrincipal user);
    Task DeleteCurrentAsync(int id, ClaimsPrincipal user);
}

public class ReservationService : GenericService<Reservation, ReservationForAdminDto>, IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IReservationRoomTypeRepository _reservationRoomTypeRepository;
    private readonly IReservationRoomTypeService _reservationRoomTypeService;
    private readonly IRoomTypeService _roomTypeService;
    private readonly UserContext _userContext;
    
    public ReservationService(IReservationRepository reservationRepository, IClientRepository clientRepository, 
        IReservationRoomTypeRepository reservationRoomTypeRepository, IReservationRoomTypeService reservationRoomTypeService,
        IRoomTypeService roomTypeService, UserContext userContext) : base (reservationRepository)
    {
        _reservationRepository = reservationRepository;
        _clientRepository = clientRepository;
        _reservationRoomTypeRepository = reservationRoomTypeRepository;
        _reservationRoomTypeService = reservationRoomTypeService;
        _roomTypeService = roomTypeService;
        _userContext = userContext;
    }

    public async Task<IEnumerable<ReservationForAdminDto>> GetAllWithRoomTypesAsync()
    {
        var reservations = await _reservationRepository.GetAllWithRoomTypesAsync();

        var reservationsDto = reservations.Adapt<IEnumerable<ReservationForAdminDto>>();
        
        return reservationsDto;
    }

    public async Task<ReservationForAdminDto> GetByIdWithRoomTypesAsync(int id)
    {
        var reservation = await _reservationRepository.GetByIdWithRoomTypesAsync(id);

        if (reservation == null)
            throw new NotFoundException("Reservation not found");
        
        var reservationDto = reservation.Adapt<ReservationForAdminDto>();
        
        return reservationDto;
    }

    public async Task<IEnumerable<ReservationForClientDto>> GetAllCurrentAsync(ClaimsPrincipal user)
    {
        var userId = int.Parse(_userContext.UserId);
        
        var client = await _clientRepository.GetByUserIdAsync(userId);

        if (client == null)
            throw new NotFoundException("Client not found");
        
        var reservations = await _reservationRepository.GetAllByClientAsync(client.Id);
        
        var reservationsDto = reservations.Adapt<IEnumerable<ReservationForClientDto>>();
        
        return reservationsDto;
    }

    public async Task<ReservationForClientDto> GetCurrentByIdAsync(int id, ClaimsPrincipal user)
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

    public async Task CreateAsync(ReservationForAdminDto reservation)
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
        
        await _reservationRepository.CreateReservationWithTransactionAsync(newReservation, reservation.RoomTypes);
        
        await _reservationRoomTypeService.AddAsync(newReservation.Id, reservation.RoomTypes);
    }

    public async Task CreateCurrentAsync(ClaimsPrincipal user, ReservationForClientCreateDto reservation)
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
        newReservation.TotalPrice = await CalculateTotalPrice(reservation);
        
        await _reservationRepository.CreateReservationWithTransactionAsync(newReservation, reservation.RoomTypes);
        
        await _reservationRoomTypeService.AddAsync(newReservation.Id, reservation.RoomTypes);
    }

    public async Task UpdateAsync(int id, ReservationForAdminDto reservationForAdminDto)
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
        
        await _reservationRoomTypeService.UpdateAsync(oldReservation.Id, reservationForAdminDto.RoomTypes);
    }

    public async Task UpdateCurrentAsync(int id, ReservationForClientUpdateDto reservationForClientUpdateDto, ClaimsPrincipal user)
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

    public async Task DeleteCurrentAsync(int id, ClaimsPrincipal user)
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

    private async Task<decimal> CalculateTotalPrice(ReservationForClientCreateDto reservation)
    {
        decimal totalPrice = 0;
        
        int totalDays = (reservation.CheckOutDate - reservation.CheckInDate).Days;

        foreach (var roomType in reservation.RoomTypes)
        {
            decimal roomPrice = await _roomTypeService.GetPriceAsync(roomType.RoomTypeId);
            totalPrice += roomPrice * roomType.ReservedRoomCount * totalDays;
        }
        
        return totalPrice;
    }
}