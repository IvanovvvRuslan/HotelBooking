using HotelBooking.Exceptions;
using HotelBooking.Repositories;
using Mapster;

namespace HotelBooking.Services;

public interface IGenericService<TEntity, TDto>
    where TEntity : class
    where TDto : class
{
    Task<IEnumerable<TDto>> GetAllAsync();
    Task<TDto> GetByIdAsync(int id);
    Task DeleteAsync(int id);
}

public class GenericService<TEntity, TDto> : IGenericService<TEntity, TDto>
    where TEntity : class
    where TDto : class
{
    protected readonly IGenericRepository<TEntity> _repository;

    public GenericService(IGenericRepository<TEntity> repository)
    {
        _repository = repository;
    }


    public async Task<IEnumerable<TDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();

        var entitiesDto = entities.Adapt<IEnumerable<TDto>>();
        
        return entitiesDto;
    }

    public async Task<TDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);

        if (entity == null)
            throw new NotFoundException($"{typeof(TEntity).Name} not found");
        
        var entityDto = entity.Adapt<TDto>();
        
        return entityDto;
    }
    
    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);

        if (entity == null)
            throw new NotFoundException($"{typeof(TEntity).Name} type not found");
        
        _repository.Delete(entity);
        await _repository.SaveChangesAsync();
    }
}