using System.Security.Claims;
using HotelBooking.DTO;
using HotelBooking.DTO.ResponseDto;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using HotelBooking.Repositories;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace HotelBooking.Services;

public interface IClientService : IGenericService<Client, ClientForAdminDto>
{
    Task<ClientForUserDto> GetCurrentAsync(ClaimsPrincipal user);
    Task CreateAsync(SignUpDto signUpDto);
    Task UpdateAsync(int id, ClientForAdminDto clientForAdminDto);
    Task UpdateCurrentAsync(ClaimsPrincipal user, ClientForUserDto clientForUserDto);
    Task DeleteWithUserAsync(int id);
    Task DeleteCurrentWithUserAsync(ClaimsPrincipal user);
}

public class ClientService : GenericService<Client, ClientForAdminDto>, IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly UserManager<User> _userManager;
    private readonly IUserContext _userContext;
    private readonly IUserService _userService;
    
    public ClientService(IClientRepository clientRepository, UserManager<User> userManager, IUserContext userContext,
        IUserService userService) : base(clientRepository)
    {
        _clientRepository = clientRepository;
        _userManager = userManager;
        _userContext = userContext;
        _userService = userService;
    }

    public async Task<ClientForUserDto> GetCurrentAsync(ClaimsPrincipal user)
    {
        var userId = int.Parse(_userContext.UserId);
        
        var client = await _clientRepository.GetByUserIdAsync(userId);
        
        if (client == null)
            throw new NotFoundException("Client not found");
        
        var clientDto = client.Adapt<ClientForUserDto>();
        
        var clientUser = await _userManager.FindByIdAsync(userId.ToString());
        
        if (clientUser == null)
            throw new NotFoundException("User not found");
        
        clientDto.PhoneNumber = clientUser.PhoneNumber;
        
        return clientDto;
    }

    public async Task CreateAsync(SignUpDto signUpDto)
    {
        bool isAdmin = false;
        
        await _userService.CreateAsync(signUpDto, isAdmin);
    }

    public async Task UpdateAsync(int id, ClientForAdminDto clientForAdminDto)
    {
        var oldClient = await _clientRepository.GetByIdTrackedAsync(id);

        if (oldClient == null)
            throw new NotFoundException("Client not found");
        
        oldClient.Gender = clientForAdminDto.Gender;
        oldClient.Country = clientForAdminDto.Country;
        
        var user = await _userManager.FindByIdAsync(clientForAdminDto.UserId.ToString());
        
        if (user == null)
            throw new NotFoundException("User not found");
        
        user.PhoneNumber = clientForAdminDto.PhoneNumber;
        
        await _clientRepository.SaveChangesAsync();
    }

    public async Task UpdateCurrentAsync(ClaimsPrincipal user, ClientForUserDto clientForUserDto)
    {
        var userId = int.Parse(_userContext.UserId);

        var oldClient = await _clientRepository.GetByUserIdTrackedAsync(userId);
        
        if (oldClient == null)
            throw new NotFoundException("Client not found");
        
        oldClient.Gender = clientForUserDto.Gender;
        oldClient.Country = clientForUserDto.Country;
        
        var clientUser = await _userManager.FindByIdAsync(userId.ToString());

        if (clientUser == null)
            throw new NotFoundException("User not found");
        
        clientUser.PhoneNumber = clientForUserDto.PhoneNumber;
        
        await _clientRepository.SaveChangesAsync();
    }

    public async Task DeleteWithUserAsync(int id)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        
        if (client == null)
            throw new NotFoundException("Client not found");
        
        var user = await _userManager.FindByIdAsync(client.UserId.ToString());
        
        if (user != null)
            await _userManager.DeleteAsync(user);
        
        client = await _clientRepository.GetByIdTrackedAsync(id);
        
        if (client != null)
        {
            _clientRepository.Delete(client);
            await _clientRepository.SaveChangesAsync();    
        }
    }

    public async Task DeleteCurrentWithUserAsync(ClaimsPrincipal user)
    {
        var userId = int.Parse(_userContext.UserId);
        
        var client = await _clientRepository.GetByUserIdAsync(userId);
        
        if (client == null)
            throw new NotFoundException("Client not found");
        
        var clientUser = await _userManager.FindByIdAsync(userId.ToString());
        
        if (clientUser != null)
            await _userManager.DeleteAsync(clientUser);
        
        client = await _clientRepository.GetByUserIdAsync(userId);

        if (client != null)
        {
            _clientRepository.Delete(client);
            await _clientRepository.SaveChangesAsync();
        }
    }
}

