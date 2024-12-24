using FactoryApp;

FactoryService factoryService = new FactoryService();
List<Task> tasks = new List<Task>();

for (int i = 0; i < 100; i++)
{
    tasks.Add(Task.Run(() => factoryService.AddParts(1)));
    tasks.Add(Task.Run(() => factoryService.TakeParts(1)));
}

await Task.WhenAll(tasks);
Console.WriteLine("Done!");
