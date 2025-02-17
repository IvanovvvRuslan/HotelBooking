using System.Collections;
using HotelBooking.DTO.ResponseDto;
using HotelBooking.Exceptions;
using HotelBooking.Models;
using HotelBooking.Repositories;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace HotelBooking.Services;

public interface IClientService
{
    Task<IEnumerable<ClientDto>> GetAllClientsAsync();
    Task<ClientDto> GetClientById(int id);
    Task CreateClientAsync(ClientDto clientDto);
    Task UpdateClientAsync(int id, ClientDto clientDto);
    Task DeleteClientAsync(int id);
}

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly UserManager<User> _userManager;

    public ClientService(IClientRepository clientRepository, UserManager<User> userManager)
    {
        _clientRepository = clientRepository;
        _userManager = userManager;
    }


    public async Task<IEnumerable<ClientDto>> GetAllClientsAsync()
    {
        var clients = await _clientRepository.GetallAsync();
        
        var clientsDto = clients.Adapt<IEnumerable<ClientDto>>();
        
        return clientsDto;
    }

    public async Task<ClientDto> GetClientById(int id)
    {
        var client = await _clientRepository.GetByIdAsync(id);

        if (client == null)
            throw new NotFoundException("Client not found");
        
        var clientDto = client.Adapt<ClientDto>();
        
        return clientDto;
    }

    public async Task CreateClientAsync(ClientDto clientDto)
    {
        var newClient = new Client
        {
            UserId = clientDto.UserId,
            RegistrationDate = DateTime.UtcNow,
            Gender = clientDto.Gender,
            Country = clientDto.Country,
            IsVip = clientDto.isVip
        };
        
        //Save PhoneNumber to User
        var user = await _userManager.FindByIdAsync(clientDto.UserId.ToString());
        
        if (user == null)
            throw new NotFoundException("User not found");
        
        user.PhoneNumber = clientDto.PhoneNumber;
        
        await _clientRepository.CreateAsync(newClient);
        await _clientRepository.SaveChangesAsync();
        await _userManager.UpdateAsync(user);
    }

    public async Task UpdateClientAsync(int id, ClientDto clientDto)
    {
        var oldClient = await _clientRepository.GetByIdTrackedAsync(id);

        if (oldClient == null)
            throw new NotFoundException("Client not found");
        
        oldClient.Gender = clientDto.Gender;
        oldClient.Country = clientDto.Country;
        oldClient.User.PhoneNumber = clientDto.PhoneNumber;
        
        await _clientRepository.SaveChangesAsync();
    }

    public async Task DeleteClientAsync(int id)
    {
        var client = await _clientRepository.GetByIdAsync(id);

        if (client == null)
            throw new NotFoundException("Client not found");
        
        await _clientRepository.DeleteAsync(client);
        await _clientRepository.SaveChangesAsync();
    }
}