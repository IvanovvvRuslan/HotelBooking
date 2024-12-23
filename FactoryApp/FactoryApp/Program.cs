using FactoryApp;

FactoryService factoryService = new FactoryService();
Task[] tasks = new Task[100];

for (int i = 0; i < 100; i++)
{
    tasks[i] = Task.Run(() => factoryService.AddParts(1));
    tasks[i] = Task.Run(() => factoryService.TakeParts(1));
    
}

await Task.WhenAll(tasks);
Console.WriteLine("Done!");
