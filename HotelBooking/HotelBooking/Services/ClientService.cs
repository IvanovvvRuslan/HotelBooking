using System.Collections;
using System.Security.Claims;
using HotelBooking.DTO;
using HotelBooking.DTO.ResponseDto;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using HotelBooking.Repositories;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace HotelBooking.Services;

public interface IClientService
{
    Task<IEnumerable<ClientForAdminDto>> GetAllClientsAsync();
    Task<ClientForAdminDto> GetClientByIdAsync(int id);
    Task<ClientForUserDto> GetCurrentClientAsync(ClaimsPrincipal user);
    Task CreateClientAsync(SignUpDto signUpDto);
    Task UpdateClientAsync(int id, ClientForAdminDto clientForAdminDto);
    Task UpdateCurrentClientAsync(ClaimsPrincipal user, ClientForUserDto clientForUserDto);
    Task DeleteClientAsync(int id);
    Task DeleteCurrentClientAsync(ClaimsPrincipal user);
}

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly UserManager<User> _userManager;
    private readonly UserContext _userContext;
    private readonly IUserService _userService;
    
    public ClientService(IClientRepository clientRepository, UserManager<User> userManager, UserContext userContext,
        IUserService userService)
    {
        _clientRepository = clientRepository;
        _userManager = userManager;
        _userContext = userContext;
        _userService = userService;
    }


    public async Task<IEnumerable<ClientForAdminDto>> GetAllClientsAsync()
    {
        var clients = await _clientRepository.GetAllAsync();
        
        var clientsDto = clients.Adapt<IEnumerable<ClientForAdminDto>>();
        
        return clientsDto;
    }

    public async Task<ClientForAdminDto> GetClientByIdAsync(int id)
    {
        var client = await _clientRepository.GetByIdAsync(id);

        if (client == null)
            throw new NotFoundException("Client not found");
        
        var clientDto = client.Adapt<ClientForAdminDto>();
        
        return clientDto;
    }

    public async Task<ClientForUserDto> GetCurrentClientAsync(ClaimsPrincipal user)
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

    public async Task CreateClientAsync(SignUpDto signUpDto)
    {
        bool isAdmin = false;
        
        await _userService.CreateUserAsync(signUpDto, isAdmin);
    }

    public async Task UpdateClientAsync(int id, ClientForAdminDto clientForAdminDto)
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

    public async Task UpdateCurrentClientAsync(ClaimsPrincipal user, ClientForUserDto clientForUserDto)
    {
        var userId = int.Parse(_userContext.UserId);

        var oldClient = await _clientRepository.GetByUserIdTrackedAsync(userId);
        
        oldClient.Gender = clientForUserDto.Gender;
        oldClient.Country = clientForUserDto.Country;
        
        var clientUser = await _userManager.FindByIdAsync(userId.ToString());

        if (clientUser == null)
            throw new NotFoundException("User not found");
        
        clientUser.PhoneNumber = clientForUserDto.PhoneNumber;
        
        await _clientRepository.SaveChangesAsync();
    }

    public async Task DeleteClientAsync(int id)
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
            await _clientRepository.Delete(client);
            await _clientRepository.SaveChangesAsync();    
        }
    }

    public async Task DeleteCurrentClientAsync(ClaimsPrincipal user)
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
            await _clientRepository.Delete(client);
            await _clientRepository.SaveChangesAsync();
        }
    }
}

