using FactoryApp;

FactoryService factoryService = new FactoryService();
List<Task> tasks = new List<Task>();

for (int i = 0; i < 100; i++)
{
    tasks.Add(factoryService.AddPartsAsync(1));
    tasks.Add(factoryService.TakePartsAsync(1));
}

await Task.WhenAll(tasks);
Console.WriteLine("Done!");
