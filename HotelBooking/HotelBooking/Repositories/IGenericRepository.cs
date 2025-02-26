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