using HotelBooking.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdTrackedAsync(int id);
    Task CreateAsync(T entity);
    Task Delete(T entity);
    Task SaveChangesAsync();
}

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
    
    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<T?> GetByIdTrackedAsync(int id)
    {
        return await _dbSet
            .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
    }

    public async Task CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public Task Delete(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}