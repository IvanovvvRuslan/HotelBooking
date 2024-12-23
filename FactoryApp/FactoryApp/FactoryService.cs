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
    
    Task[] tasks = new Task[100];
    
    
    public async Task AddParts(int amount)
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

    public async Task TakeParts(int amount)
    {
        while (_factory.PartsInStock > 0)
        {
            try
            {
                _factory.PartsInStock -= amount;
                Console.WriteLine($"{amount} detail taken. Total amount: {_factory.PartsInStock}");
            }
            finally
            {
                await _semaphore.WaitAsync();
            }
        }
    }
}