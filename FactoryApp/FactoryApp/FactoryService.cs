namespace FactoryApp;

public class FactoryService
{
    private readonly FactoryModel _factory;
    private readonly SemaphoreSlim _semaphore;
    
    public FactoryService()
    {
        _factory = new FactoryModel();
        _semaphore = new SemaphoreSlim(1, 1);
    }
    
    public async Task AddPartsAsync(int amount)
    {
        await _semaphore.WaitAsync();
        try
        {
            _factory.PartsInStock += amount;
            Console.WriteLine($"{amount} detail added. Total amount: {_factory.PartsInStock}");
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task TakePartsAsync(int amount)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_factory.PartsInStock > 0)
            {
                _factory.PartsInStock -= amount;
                 Console.WriteLine($"{amount} detail taken. Total amount: {_factory.PartsInStock}");
            }
            else
            {
                Console.WriteLine("No more available part to take");
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
}